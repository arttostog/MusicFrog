using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace MusicFrog
{
    internal class AppNotificationClient : IMMNotificationClient
    {
        private readonly AppVolume _appVolume;
        private readonly MMDeviceEnumerator _deviceEnumerator = new MMDeviceEnumerator();

        public AppNotificationClient(AppVolume appVolume) =>
            _appVolume = appVolume;

        public void OnDeviceStateChanged(string deviceId, DeviceState newState) { }

        public void OnDeviceAdded(string pwstrDeviceId) { }

        public void OnDeviceRemoved(string deviceId) { }

        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
        {
            if (flow == DataFlow.Render && role == Role.Multimedia)
                _appVolume.Device = _deviceEnumerator.GetDefaultAudioEndpoint(flow, role).AudioMeterInformation;
        }

        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key) { }
    }
}
