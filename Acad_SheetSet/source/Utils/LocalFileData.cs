namespace Acad_SheetSet.Utils
{
    using System;
    using System.IO;

    /// <summary>
    /// Данные хранимые в файле json локально
    /// </summary>
    public class LocalFileData<T>
        where T : class, new()
    {
        public readonly string LocalFile;
        private readonly bool isXmlOrJson;
        private DateTime fileLastWrite;

        public T? Data { get; set; }

        /// <summary>
        /// Данные хранимые в файле json локально
        /// </summary>
        /// <param name="localFile"></param>
        /// <param name="isXmlOrJson">true - xml, false - json.</param>
        public LocalFileData(string localFile, bool isXmlOrJson)
        {
            LocalFile = localFile;
            this.isXmlOrJson = isXmlOrJson;
        }

        public bool HasChanges()
        {
            return File.GetLastWriteTime(LocalFile) > fileLastWrite;
        }

        /// <summary>
        /// Load
        /// </summary>
        public void Load()
        {
            Data = Deserialize();
            fileLastWrite = File.GetLastWriteTime(LocalFile);
        }

        /// <summary>
        /// Load
        /// </summary>
        public T? TryLoadData()
        {
            try
            {
                Data = Deserialize();
                fileLastWrite = File.GetLastWriteTime(LocalFile);
                return Data;
            }
            catch
            {
                return Data;
            }
        }

        public void Save()
        {
            Serialize();
        }

        public void TryLoad(Func<T> onError)
        {
            try
            {
                Load();
            }
            catch (Exception ex)
            {
                Data = onError();
            }
        }

        public void TryLoad()
        {
            try
            {
                Load();
            }
            catch (FileNotFoundException)
            {
                // Файл не найден
            }
            catch (Exception ex)
            {
            }
        }

        public void TrySave()
        {
            try
            {
                Save();
            }
            catch (Exception ex)
            {
            }
        }

        private T Deserialize()
        {
            if (!File.Exists(LocalFile))
                throw new FileNotFoundException(LocalFile);
            return isXmlOrJson ? SerializerXml.Load<T>(LocalFile) : LocalFile.Deserialize<T>();
        }

        private void Serialize()
        {
            if (isXmlOrJson) SerializerXml.Save(LocalFile, Data);
            else Data.Serialize(LocalFile);
        }
    }
}