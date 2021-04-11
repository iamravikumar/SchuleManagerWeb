namespace SchuleManager.Models
{
    /// <summary>
    /// Image object class.
    /// </summary>
    public class PhotoObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets Image ID.
        /// </summary>
        public int FileId { get; set; }

        /// <summary>
        /// Gets or sets Image name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets Image extension.
        /// </summary>
        public string FileContentType { get; set; }

        #endregion
    }
}