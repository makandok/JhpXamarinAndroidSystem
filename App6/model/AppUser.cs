namespace JhpDataSystem.model
{
    public class AppUser : ISaveableEntity
    {
        public KindKey Id { get; set; }
        public string UserId { get; set; }
        public string Names { get; set; }
        public string KnownBolg { get; set; }
        //public string MotherOfBolg { get; set; }
    }

    public class UserSession
    {
        public KindKey Id { get; set; }
        public string AuthorisationToken { get; set; }
        public AppUser User { get; set; }
    }
}