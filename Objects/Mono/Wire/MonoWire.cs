using TheElectrician.Models;
using TheElectrician.Objects.Mono.Wire;
using TheElectrician.Systems.Config;
using TheElectrician.Systems.PowerFlow;

namespace TheElectrician.Objects.Mono;

public class MonoWire : ElectricMono, Hoverable, Interactable
{
    private Transform cablesParent;
    public IWire wire { get; private set; }

    public override string GetHoverText()
    {
        StringBuilder sb = new();
        sb.AppendLine(piece.m_name.Localize());
        if (m_debugMode)
        {
            sb.AppendLine($"ID: {wire.GetId()}");
            sb.AppendLine($"State: {wire.GetState()}");
            var connected = wire.GetConnections().Select(x => x?.GetId().ToString() ?? "null").ToList();
            sb.AppendLine($"Connected: {(connected.Count > 0 ? connected.GetString() : "none")}");
        }

        sb.AppendLine();
        var powerInSystem = PowerFlow.GetPowerInSystem(wire);
        if (powerInSystem != -1)
            sb.AppendLine(string.Format($"${ModName}_power_in_system".Localize(), powerInSystem));
        else
            sb.AppendLine($"${ModName}_wire_out_of_power_system".Localize());
        sb.AppendLine($"[<color=yellow><b>E</b></color>] ${ModName}_wire_connect".Localize());
        sb.AppendLine($"[<color=yellow><b>$button_lshift + E</b></color>] ${ModName}_wire_disconnect"
            .Localize());

        return sb.ToString();
    }

    public bool Interact(Humanoid user, bool hold, bool alt)
    {
        if (hold) return false;
        if (alt)
        {
            DisconnectMode();
            return true;
        }

        ConnectMode();
        return true;
    }


    public bool UseItem(Humanoid user, ItemData item) { return false; }

    public override Guid GetId()
    {
        if (wire == null || !wire.IsValid()) return Guid.Empty;
        return wire.GetId();
    }

    public override void SetUp()
    {
        base.SetUp();
        if (!netView.IsValid()) return;
        cablesParent = transform.FindChildByName("CablesAttach");
        if (cablesParent == null)
        {
            DebugWarning("Cables parent not found");
            cablesParent = new GameObject("Cables").transform;
            cablesParent.SetParent(transform);
            cablesParent.position = gameObject.GetTopPosition();
        }
    }

    public override void Load()
    {
        wire = Library.GetObject(netView.GetZDO()) as IWire;
        if (wire is null)
        {
            DebugError($"Wire {netView.GetZDO()} not found");
            foreach (var child in gameObject.GetComponentsInChildren<Renderer>()) child.material.color = Color.red;
            return;
        }

        wire.onConnectionsChanged.AddListener(UpdateCables);
        StartCoroutine(UpdateCablesIEnumerator());
    }

    private IEnumerator UpdateCablesIEnumerator()
    {
        yield return new WaitForSeconds(TheConfig.WireUpdateCableInterval);
        try
        {
            UpdateCables();
        }
        catch (Exception e)
        {
            DebugError($"Failed to update cables: {e}");
        }

        StartCoroutine(UpdateCablesIEnumerator());
    }

    private void UpdateCables()
    {
        for (var i = 0; i < cablesParent.childCount; i++)
            Destroy(cablesParent.GetChild(i).gameObject);
        var connectedTo = wire.GetConnections();
        if (connectedTo.Count == 0)
        {
            cablesParent.gameObject.SetActive(false);
            return;
        }

        cablesParent.gameObject.SetActive(true);

        foreach (var connectedWire in connectedTo)
        {
            if (connectedWire is null) continue;
            var cable = new GameObject($"Cable to {connectedWire.GetId()}").AddComponent<Cable>();
            var cableTransform = cable.transform;
            cableTransform.SetParent(cablesParent);
            cableTransform.localPosition = Vector3.zero;

            var first = Library.GetObject(wire.GetId()) as IWireConnectable;
            var second = Library.GetObject(connectedWire.GetId()) as IWireConnectable;

            cable.SetConnection(first, second);

            //TODO: Attach cable on the CablesAttach
        }
    }

    private void ConnectMode()
    {
        var wires = Library.GetAllObjects<IWire>().FindAll(x => x.GetState() == WireState.Connecting);
        if (wires.Count > 1)
        {
            DebugError($"There are {wires.Count} connecting to other wire wires. This should not happen");
            wires.ForEach(x => x.SetState(WireState.Idle));
            return;
        }

        var connectingWire = wires.FirstOrDefault();
        if (connectingWire == null)
        {
            wire.SetState(WireState.Connecting);
            return;
        }

        if (connectingWire == wire)
        {
            DebugError("Cannot connect wire to itself");
            m_localPlayer?.Message(MessageHud.MessageType.TopLeft,
                "<color=#F33F37>Cannot connect wire to itself</color>");
            wire.SetState(WireState.Idle);
            return;
        }

        connectingWire.AddConnection(wire);
        wire.SetState(WireState.Idle);
        connectingWire.SetState(WireState.Idle);
        m_localPlayer.Message(MessageHud.MessageType.TopLeft, "<color=#95E455>Connected</color>");
    }

    private void DisconnectMode()
    {
        var wires = Library.GetAllObjects<IWire>().FindAll(x => x.GetState() == WireState.Disconnecting);
        if (wires.Count > 1)
        {
            DebugError($"There are {wires.Count} disconnecting from other wire wires. This should not happen");
            wires.ForEach(x => x.SetState(WireState.Idle));
            return;
        }

        var connectingWire = wires.FirstOrDefault();
        if (connectingWire == null)
        {
            wire.SetState(WireState.Disconnecting);
            return;
        }

        if (connectingWire == wire)
        {
            foreach (var connection in wire.GetConnections()) wire.RemoveConnection(connection);
            return;
        }

        connectingWire.RemoveConnection(wire);
        wire.SetState(WireState.Idle);
        connectingWire.SetState(WireState.Idle);
        m_localPlayer?.Message(MessageHud.MessageType.TopLeft, "<color=#95E455>Disconnected</color>");
    }
}