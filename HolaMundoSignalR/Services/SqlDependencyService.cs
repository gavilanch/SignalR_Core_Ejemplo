using HolaMundoSignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HolaMundoSignalR.Services
{
    public interface IDatabaseChangeNotificationService
    {
        void Config();
    }

    public class SqlDependencyService : IDatabaseChangeNotificationService
    {
        private readonly IConfiguration configuration;
        private readonly IHubContext<ChatHub> chatHub;

        public SqlDependencyService(IConfiguration configuration,
            IHubContext<ChatHub> chatHub)
        {
            this.configuration = configuration;
            this.chatHub = chatHub;
        }

        public void Config()
        {
            SuscribirseALosCambiosDeLaTablaPersonas();
        }

        private void SuscribirseALosCambiosDeLaTablaPersonas()
        {
            string connString = configuration.GetConnectionString("DefaultConnection");
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                //No funciona si usas Select *
                using (var cmd = new SqlCommand(@"SELECT Nombre FROM [dbo].Personas", conn))
                {
                    cmd.Notification = null;
                    SqlDependency dependency = new SqlDependency(cmd);
                    dependency.OnChange += Personas_Cambio;
                    SqlDependency.Start(connString);
                    cmd.ExecuteReader(); // Hay que correr el query
                }
            }
        }
        private void Personas_Cambio(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                string mensaje = ObtenerMensajeAMostrar(e);
                chatHub.Clients.All.SendAsync("ReceiveMessage", "Admin", mensaje);
            }

            SuscribirseALosCambiosDeLaTablaPersonas(); // Importante: Hay que volver a suscribirse

        }

        private string ObtenerMensajeAMostrar(SqlNotificationEventArgs e)
        {
            switch (e.Info)
            {
                case SqlNotificationInfo.Insert:
                    return "Un registro ha sido insertado";
                case SqlNotificationInfo.Delete:
                    return "Un registro ha sido borrado";
                case SqlNotificationInfo.Update:
                    return "Un registro ha sido actualizado";
                default:
                    return "Un cambio desconocido ha ocurrido";
            }
        }



    }


}
