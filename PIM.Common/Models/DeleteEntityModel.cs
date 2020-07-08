namespace PIM.Common.Models
{
    public class DeleteEntityModel
    {
        #region Constructors
        public DeleteEntityModel() { }
        public DeleteEntityModel(int id, int version)
        {
            Id = id;
            Version = version;
        }
        #endregion
        #region Properties
        public int Id { get; set; }
        public int Version { get; set; }
        #endregion
    }
}
