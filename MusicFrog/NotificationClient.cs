using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace MusicFrog
{
    class NotificationClient : IMMNotificationClient
    {
        private readonly MusicFrogApp App;

        public NotificationClient(MusicFrogApp App) { this.App = App; }

        public void OnDeviceStateChanged(string deviceId, DeviceState newState) { }

        public void OnDeviceAdded(string pwstrDeviceId) { }

        public void OnDeviceRemoved(string deviceId) { }

        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
        {
            App.Device = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia).AudioMeterInformation;
        }

        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key) { }
    }
}
