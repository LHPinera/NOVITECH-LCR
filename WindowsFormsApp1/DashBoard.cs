using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Impinj.OctaneSdk;

namespace RepMovilLeeChips
{
    public partial class DashBoard : Form
    {
        static ImpinjReader reader = new ImpinjReader();
        static int _chips;
        static string SQLconstr = @"Data Source=CYBOLT-RPV-SRV\SQLEXPRESS;Initial Catalog=arcomovilrepuve;User Id=sa;Password=Admin123";
        static string Arco;
        static int ArcoId;
        static string hostname;

        public DashBoard()
        {
            InitializeComponent();
            var appSettings = ConfigurationManager.AppSettings;
            CheckForIllegalCrossThreadCalls = false;
            _chips = 0;
            Arco = appSettings["Arco"];
            hostname = appSettings["IPLector"];
            try
            {
                reader.Connect(hostname);
                Settings settings = reader.QueryDefaultSettings();
                settings.Report.IncludeAntennaPortNumber = true;
                settings.ReaderMode = ReaderMode.AutoSetDenseReader;
                settings.SearchMode = SearchMode.SingleTarget;
                settings.Session = 2;
                settings.Antennas.EnableAll();
                settings.Antennas.TxPowerMax = true;
                settings.Antennas.RxSensitivityMax = true;
                //settings.Antennas.GetAntenna(1).IsEnabled = true;
                //settings.Antennas.GetAntenna(1).MaxTxPower = true;
                //settings.Antennas.GetAntenna(1).MaxRxSensitivity = true;
                reader.ApplySettings(settings);
                reader.TagsReported += OnTagsReported;

                tbFecha.ReadOnly = true;
                tbAntena.ReadOnly = true;
                tbFolio.ReadOnly = true;
                tbTotal.ReadOnly = true;

                btnDetener.Enabled = true;
                btnIniciar.Enabled = false;

                reader.Start();


            }
            catch (OctaneSdkException e)
            {
                Console.WriteLine("Octane SDK excepción {0}", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Excepción {0}", e.Message);
            }
        }


        private void OnTagsReported(ImpinjReader sender, TagReport report)
        {
            Tag tag = report.Tags.Last();

            Lectura lectura = new Lectura()
            {
                Fecha = DateTime.Now,
                Antena = tag.AntennaPortNumber.ToString(),
                Folio = ConvertHex(tag.Epc.ToHexString()),
                Contador = ++_chips
            };

            ShowData(lectura);
            GrabaChip(lectura);


        }

        private string ConvertHex(String hexString)
        {
            try
            {
                string ascii = string.Empty;

                for (int i = 0; i < hexString.Length; i += 2)
                {
                    String hs = string.Empty;

                    hs = hexString.Substring(i, 2);
                    uint decval = System.Convert.ToUInt32(hs, 16);
                    char character = System.Convert.ToChar(decval);
                    ascii += character;

                }

                return ascii;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return string.Empty;
        }

        private void ShowData(Lectura lectura)
        {
            tbFecha.Text = lectura.Fecha.ToShortDateString() + " - "+lectura.Fecha.ToShortTimeString();
            tbAntena.Text = lectura.Antena;
            tbFolio.Text = lectura.Folio;
            tbTotal.Text = lectura.Contador.ToString();
        }

        private void Detener(object sender, EventArgs e)
        {
            try
            {
                reader.Stop();
                btnDetener.Enabled = false;
                btnIniciar.Enabled = true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Excepcion {0}", ex.Message);
            }
        }

        private void Iniciar(object sender, EventArgs e)
        {
            try
            {
                reader.Start();
                btnIniciar.Enabled = false;
                btnDetener.Enabled = true;
                tbFecha.Text = "";
                tbAntena.Text = "";
                tbFolio.Text = "";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Excepcion {0}", ex.Message);
            }
        }

        private void GrabaChip(Lectura lectura)
        {
            try
            {
                SqlConnection conn = new SqlConnection(SQLconstr);
                conn.Open();
                using (conn)
                {
                    Chip NChip = new Chip()
                    {
                        Chip_Arco = Arco,
                        Chip_Arco_Id = ArcoId,
                        Chip_Fecha = lectura.Fecha,
                        Chip_Antena = lectura.Antena,
                        Chip_Folio = lectura.Folio,
                        Chip_Busqueda = 0,
                        Chip_Econtrado = 0,
                        Chip_Enviado = 0
                    };
                  
                    try
                    {
                        SqlCommand cmdInserta = new SqlCommand("INSERT INTO Chips_Leidos (Chip_Arco, Chip_Arco_Id, Chip_Fecha, Chip_Antena, Chip_Folio, Chip_Busqueda, Chip_Econtrado, Chip_Enviado) VALUES(@par1, @par2, @par3, @par4, @par5,  @par6, @par7, @par8)", conn);
                        cmdInserta.Parameters.AddWithValue("@par1", NChip.Chip_Arco);
                        cmdInserta.Parameters.AddWithValue("@par2", NChip.Chip_Arco_Id);
                        cmdInserta.Parameters.AddWithValue("@par3", NChip.Chip_Fecha);
                        cmdInserta.Parameters.AddWithValue("@par4", NChip.Chip_Antena);
                        cmdInserta.Parameters.AddWithValue("@par5", NChip.Chip_Folio);
                        cmdInserta.Parameters.AddWithValue("@par6", NChip.Chip_Busqueda);
                        cmdInserta.Parameters.AddWithValue("@par7", NChip.Chip_Econtrado);
                        cmdInserta.Parameters.AddWithValue("@par8", NChip.Chip_Enviado);
                        cmdInserta.ExecuteNonQuery();

                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex);
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                MessageBox.Show("No se encuentra el servidor de datos", "Error del red");
                Application.Exit();
            }
        }
    }
    public class Lectura
    {
        public DateTime Fecha { get; set; }
        public string Antena { get; set; }
        public string Folio { get; set; }
        public int Contador { get; set; }
    }

    public class Chip
    {
        public string Chip_Arco { get; set; }
        public int Chip_Arco_Id { get; set; }
        public DateTime Chip_Fecha { get; set; }
        public string Chip_Antena { get; set; }
        public string Chip_Folio { get; set; }
        public int Chip_Busqueda { get; set; }
        public int Chip_Econtrado { get; set; }
        public int Chip_Enviado { get; set; }

    }
}
