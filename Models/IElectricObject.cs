using TheElectrician.Settings;
using TheElectrician.Settings.Interfaces;

namespace TheElectrician.Models;

public interface IElectricObject
{
    ZDO GetZDO();
    void Update();
    void InitSettings(IElectricObjectSettings settings);
    T GetSettings<T>() where T : IElectricObjectSettings;
    IElectricObjectSettings GetSettings();
    Guid GetId();

    bool IsValid();

    void InitData();
    string ToString();
}