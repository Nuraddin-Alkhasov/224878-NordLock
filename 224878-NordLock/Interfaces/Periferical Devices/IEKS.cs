namespace HMI.Interfaces
{
    interface IEKS
    {
        void OpenConnection();
        void CloseConnection();
        string Read();
        void Write(string data);
        string GetStatus();
    }
}
