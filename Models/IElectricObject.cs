using TheElectrician.Models.Settings;

namespace TheElectrician.Models;

public interface IElectricObject
{
    ZDO GetZDO();
    void Update();
    void InitSettings(ElectricObjectSettings settings);
    Guid GetId();
}