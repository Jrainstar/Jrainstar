namespace Jrainstar
{
    public class SessionComponent : Component, IAwake
    {
        public static SessionComponent Instance { get; set; }

        private Session session;

        public Session Session
        {
            get
            {
                return this.session;
            }
            set
            {
                this.session = value;
            }
        }

        public void Awake()
        {
            Instance = this;
        }
    }
}
