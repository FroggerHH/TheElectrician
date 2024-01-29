namespace TheElectrician.Objects.Mono.Wire;

public class Cable : MonoBehaviour
{
    private Vector3 pos1;
    private Vector3 pos2;

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private LineRenderer lineRenderer;
    [SerializeField] private int _sides = 6;
    [SerializeField] private float radius = 0.05f;
    [SerializeField] private bool _useWorldSpace = true;
    [SerializeField] private float _falloffByDistance = 0.2f;

    private Vector3[] _positions;
    private float distance;
    private Mesh _mesh;
    private Vector3[] _vertices;
    private Vector3 betweenVector;

    public Material material
    {
        get => _meshRenderer.material;
        set => _meshRenderer.material = value;
    }

    private void Awake()
    {
        _mesh = new Mesh();
        _mesh.name = "Cable Mesh Srart";
        _meshFilter = gameObject.GetOrAddComponent<MeshFilter>();
        lineRenderer = gameObject.GetOrAddComponent<LineRenderer>();
        _meshRenderer = gameObject.GetOrAddComponent<MeshRenderer>();
        _meshFilter.mesh = _mesh;
        if (material is null)
        {
            material = new Material(Shader.Find("Unlit/Color"));
            material.color = new Color(0.02f, 0.11f, 0.11f);
        }
    }

    private void OnBecameVisible() => _meshRenderer.enabled = true;

    private void OnBecameInvisible() => _meshRenderer.enabled = false;

    private void LateUpdate() => _sides = Max(3, _sides);

    public void SetConnection(Vector3 obj1, Vector3 obj2)
    {
        pos1 = obj1;
        pos2 = obj2;

        transform.position = new Vector3(0, 0, 0);

        betweenVector = Vector3.Lerp(pos1, pos2, .5f);
        _falloffByDistance = Clamp(.1f * Vector3.Distance(pos1, pos2), 0f, Consts.wireMaxFalloffByDistance);
        betweenVector.y -= _falloffByDistance;
        SetPositions(
            new(pos1.x, pos1.y, pos1.z),
            betweenVector,
            new(pos2.x, pos2.y, pos2.z)
        );
    }

    private void SetPositions(params Vector3[] positions)
    {
        _positions = positions;
        distance = Vector3.Distance(pos1, pos2);
        var subDivision = 2 * (int)distance + 1;
        lineRenderer.enabled = true;
        lineRenderer.SmoothLine(_positions, subDivision, distance / 10);
        var newPos = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(newPos);
        lineRenderer.enabled = false;
        _positions = newPos;
        GenerateMesh();
    }

    private void GenerateMesh()
    {
        if (_mesh == null || _positions == null || _positions.Length <= 1)
        {
            _mesh = new Mesh();
            _mesh.name = "Cable Mesh Generated";
            return;
        }

        var verticesLength = _sides * _positions.Length;
        if (_vertices == null || _vertices.Length != verticesLength)
        {
            _vertices = new Vector3[verticesLength];

            var indices = GenerateIndices();
            var uvs = GenerateUVs();

            if (verticesLength > _mesh.vertexCount)
            {
                _mesh.vertices = _vertices;
                _mesh.triangles = indices;
                _mesh.uv = uvs;
            } else
            {
                _mesh.triangles = indices;
                _mesh.vertices = _vertices;
                _mesh.uv = uvs;
            }
        }

        var currentVertIndex = 0;

        for (var i = 0; i < _positions.Length; i++)
        {
            var circle = CalculateCircle(i);
            foreach (var vertex in circle)
                _vertices[currentVertIndex++] = _useWorldSpace ? transform.InverseTransformPoint(vertex) : vertex;
        }

        _mesh.vertices = _vertices;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();

        _meshFilter.mesh = _mesh;
    }

    private Vector2[] GenerateUVs()
    {
        var uvs = new Vector2[_positions.Length * _sides];

        for (var segment = 0; segment < _positions.Length; segment++)
        for (var side = 0; side < _sides; side++)
        {
            var vertIndex = segment * _sides + side;
            var u = side / (_sides - 1f);
            var v = segment / (_positions.Length - 1f);

            uvs[vertIndex] = new Vector2(u, v);
        }

        return uvs;
    }

    private int[] GenerateIndices()
    {
        // Two triangles and 3 vertices
        var indices = new int[_positions.Length * _sides * 2 * 3];

        var currentIndicesIndex = 0;
        for (var segment = 1; segment < _positions.Length; segment++)
        for (var side = 0; side < _sides; side++)
        {
            var vertIndex = segment * _sides + side;
            var prevVertIndex = vertIndex - _sides;

            // Triangle one
            indices[currentIndicesIndex++] = prevVertIndex;
            indices[currentIndicesIndex++] =
                side == _sides - 1 ? vertIndex - (_sides - 1) : vertIndex + 1;
            indices[currentIndicesIndex++] = vertIndex;


            // Triangle two
            indices[currentIndicesIndex++] =
                side == _sides - 1 ? prevVertIndex - (_sides - 1) : prevVertIndex + 1;
            indices[currentIndicesIndex++] =
                side == _sides - 1 ? vertIndex - (_sides - 1) : vertIndex + 1;
            indices[currentIndicesIndex++] = prevVertIndex;
        }

        return indices;
    }

    private Vector3[] CalculateCircle(int index)
    {
        var dirCount = 0;
        var forward = Vector3.zero;

        // If not first index
        if (index > 0)
        {
            forward += (_positions[index] - _positions[index - 1]).normalized;
            dirCount++;
        }

        // If not last index
        if (index < _positions.Length - 1)
        {
            forward += (_positions[index + 1] - _positions[index]).normalized;
            dirCount++;
        }

        // Forward is the average of the connecting edges directions
        forward = (forward / dirCount).normalized;
        var side = Vector3.Cross(forward, forward + new Vector3(.123564f, .34675f, .756892f)).normalized;
        var up = Vector3.Cross(forward, side).normalized;

        var circle = new Vector3[_sides];
        var angle = 0f;
        var angleStep = 2 * PI / _sides;

        var t = index / (_positions.Length - 1f);

        for (var i = 0; i < _sides; i++)
        {
            var x = Cos(angle);
            var y = Sin(angle);

            circle[i] = _positions[index] + side * x * radius + up * y * radius;

            angle += angleStep;
        }

        return circle;
    }
}