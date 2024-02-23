using TheElectrician.Settings;

namespace TheElectrician.Models;

public interface IElectricObject
{
    ZDO GetZDO();
    void Update();
    void InitSettings(ElectricObjectSettings settings);
    T GetSettings<T>() where T : ElectricObjectSettings;
    ElectricObjectSettings GetSettings();
    Guid GetId();

    bool IsValid();

    void InitData();
    string ToString();
}