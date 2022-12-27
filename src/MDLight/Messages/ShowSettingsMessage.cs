using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MDLight.Messages
{
    public class ShowSettingsMessage : ValueChangedMessage<bool>
    {
        public ShowSettingsMessage(bool value) : base(value)
        {
        }
    }
}
