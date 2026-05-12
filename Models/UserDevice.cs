namespace simple_artifacterp_back.Models
{
    public class UserDevice
    {
        // UsuarioDispositivoId
        public int UserDeviceId { get; set; }

        // UsuarioId
        public int UserId { get; set; }

        // DispositivoId
        public string? DeviceId { get; set; }

        // Plataforma
        public string? Platform { get; set; }

        // NombreDispositivo
        public string? DeviceName { get; set; }

        // PushToken
        public string? PushToken { get; set; }

        // UltimoAcceso
        public DateTime LastSeenAt { get; set; }

        // SesionActiva
        public bool IsActiveSession { get; set; }
    }
}
