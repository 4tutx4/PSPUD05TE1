using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Security.Cryptography;
using System.Collections;
using Org.BouncyCastle.Utilities.Encoders;

namespace PSPUD05TE01
{
    public partial class Form1 : Form
    {
        string [] listaFicheros;
        string[] listaPublicas;
        string usuario = "";
        public Form1()
        {
            InitializeComponent();
            if (!Directory.Exists("..\\bbdd"))
            {
                Directory.CreateDirectory("..\\bbdd");
            }
            else
            {
                this.listaFicheros = Directory.GetFiles("..\\bbdd");
            }

            if (!Directory.Exists("..\\publicKeys"))
            {
                Directory.CreateDirectory("..\\publicKeys");
            }
            else
            {
                this.listaPublicas = Directory.GetFiles("..\\publicKeys");
            }
            if (!Directory.Exists("..\\tempPrivate"))
            {
                Directory.CreateDirectory("..\\tempPrivate");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                this.groupBox2.Enabled = true;
                cargarComboBox();
            }else
            {
                this.groupBox2.Enabled = false;
            }
        }

        private void cargarComboBox()
        {
            // meter valores al combobox
            List<string> nombreContrasena = new List<string>();

            if (File.Exists("..\\bbdd\\" + this.usuario + ".txt"))
            {
                StreamReader sr = new StreamReader("..\\bbdd\\" + this.usuario + ".txt");
                string texto = sr.ReadLine();
                int cont = 0;
                while (texto != null)
                {
                    if (cont % 2 == 0)
                        nombreContrasena.Add(texto);
                    cont++;
                    texto = sr.ReadLine();
                }
                //close the file
                sr.Close();
                for (int i = 0; i < nombreContrasena.Count; i++)
                    this.comboBox2.Items.Add(nombreContrasena[i]);
            }
            else
                MessageBox.Show("El usuario no existe");
        
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked)
            {
                this.groupBox3.Enabled = true;
            }
            else
            {
                this.groupBox3.Enabled = false;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox3.Checked)
            {
                this.groupBox4.Enabled = true;

                // meter valores al combobox
                List <string> nombreContrasena= new List <string>();

                if (File.Exists("..\\bbdd\\" + this.usuario + ".txt"))
                {
                    StreamReader sr = new StreamReader("..\\bbdd\\" + this.usuario + ".txt");
                    string texto = sr.ReadLine();
                    int cont = 0;
                    while (texto != null)
                    {
                        if (cont % 2 == 0)
                            nombreContrasena.Add(texto);
                        cont++;
                        texto = sr.ReadLine();
                    }
                    //close the file
                    sr.Close();

                    for (int i = 0;i < nombreContrasena.Count;i++)
                        this.comboBox1.Items.Add(nombreContrasena[i]);
                }
                else
                    MessageBox.Show("El usuario no existe");
            }
            else
            {
                this.groupBox4.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.usuario = this.textBox1.Text;

            //con nuevo usuario se limpian todos los campos
            if(this.comboBox1.Items.Count>0)
                this.comboBox1.Text.Remove(0, this.comboBox1.Items.Count - 1);
            if (this.comboBox2.Items.Count>0)
                this.comboBox2.Text.Remove(0, this.comboBox2.Items.Count - 1);
            this.label11.Text = "Ubicación del archivo";
            this.textBox7.Text = "";
            this.textBox3.Text = "";
            this.textBox4.Text = "";



            if (!(this.listaFicheros == null))
            {
                if (!(this.listaFicheros == null) || !(this.listaFicheros.Contains("..\\bbdd\\" + this.usuario + ".txt")))
                {
                    string texto = this.textBox1.Text;
                    if (texto.Length < 4 || texto.Length > 10 || !Regex.IsMatch(texto, @"^[a-z]+$"))
                    {
                        MessageBox.Show("No cumple los requisitos");
                    }
                    else
                    {
                        this.groupBox1.Enabled = true;

                    }
                    this.checkBox1.Enabled = false;
                    this.checkBox2.Enabled = false;
                    this.checkBox3.Enabled = false;
                }
                if (this.listaFicheros.Contains("..\\bbdd\\" + this.usuario + ".txt"))
                {
                    this.groupBox1.Enabled = false;
                    this.checkBox1.Enabled = true;
                    this.checkBox2.Enabled = true;
                    this.checkBox3.Enabled = true;

                }
            }         
        }

        private void rbSi_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbSi.Checked)
            {
                this.rbNo.Checked = false;
            }
        }

        private void rbNo_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbNo.Checked)
            {
                this.rbSi.Checked = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //validar condiciones para enviar
            if (this.rbSi.Checked)
            {
                if (File.Exists("..\\bbdd\\" + this.usuario + ".txt"))
                
                    File.Delete("..\\bbdd\\" + this.usuario + ".txt");
                    
                else
                    File.Create("..\\bbdd\\" + this.usuario + ".txt");

                //crear clave privada
                crearParClaves();
                //mandar la clave por mail
                mandarMail();

                this.textBox2.Text = "";
                this.rbSi.Checked = false;
                this.checkBox1.Enabled = true;
                this.checkBox2.Enabled = true;
                this.checkBox3.Enabled = true;
                this.groupBox1.Enabled = false;


            } else
            {
                MessageBox.Show("Debe seleccionar SÍ para poder enviar");
            }
        }

        private void crearParClaves()
        {
            string publicKeyFile = "..\\publicKeys\\" + this.usuario+ "_public.xml";
            string privateKeyFile = "..\\tempPrivate\\" + this.usuario+"_private.xml";
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;

                if (File.Exists(publicKeyFile))
                    File.Delete(publicKeyFile);
                if (File.Exists(privateKeyFile))
                    File.Delete(privateKeyFile);

                string publicKey = rsa.ToXmlString(false);
                File.WriteAllText(publicKeyFile, publicKey);

                string privateKey = rsa.ToXmlString(false);
                File.WriteAllText(privateKeyFile, privateKey); 
            }
        }

        private void mandarMail()
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Borja", "batutxa@birt.eus"));
            message.To.Add(new MailboxAddress(this.usuario, this.textBox2.Text));
            message.Subject = "Clave privada";

            // Cuerpo del mensaje
            var body = new TextPart("plain")
            {
                Text = @"Hola.
                        Adjunto se incluye un fichero con la clave privada.
                        Un saludo."
            };
            //archivo adjunto
            var attachment = new MimePart("application", "octet-stream")
            {
                Content = new MimeContent(File.OpenRead("..\\tempPrivate\\"+this.usuario+"_private.xml")),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = "private.txt"
            };
            // Crear el mensaje de cuerpo con adjunto
            var multipart = new Multipart("mixed");
            multipart.Add(body);
            multipart.Add(attachment);

            message.Body = multipart;

            // Configuración del cliente SMTP
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                // Conexión al servidor SMTP de Gmail
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // Autenticación
                client.Authenticate("batutxa@birt.eus", "XTiQN5sE");

                // Enviar el correo
                client.Send(message);

                // Desconectar y liberar recursos
                client.Disconnect(true);

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (this.textBox4 != null)
            {
                if (comprobarContrasena())
                {
                    String contrasena = cifrarContrasena();
                    if (this.textBox3 != null)
                    {
                        escribirFichero(contrasena);
                        this.textBox4.Text = "";
                        MessageBox.Show("Contraseña guardada correctamente");
                    }
                    else
                        MessageBox.Show("Debe introducir una descripción");

                }
                else MessageBox.Show("La contraseña no cumple los requisitos");

            }
        }

        private bool comprobarContrasena()
        {
            string contrasena = this.textBox4.Text;
            string patron = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#&()–[\]{}:',?/*~$^+=<>]).{8,10}$";

            Console.WriteLine(contrasena);

            if (Regex.IsMatch(contrasena, patron))
            {
                return true;
            }
            return false;
        }

        private string cifrarContrasena ()
        {
            byte[] encriptado;

            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                string publicKey = File.ReadAllText("..\\publicKeys\\" + this.usuario + "_public.xml");
                rsa.FromXmlString(publicKey);
                encriptado = rsa.Encrypt(Encoding.UTF8.GetBytes(this.textBox4.Text),true);
            }
                return BitConverter.ToString(encriptado);
        }

        private void escribirFichero(string contrasena)
        {
            if (File.Exists("..\\bbdd\\" + usuario + ".txt"))
            {
                try
                {
                    using (StreamWriter sw = File.AppendText("..\\bbdd\\" + usuario + ".txt"))
                    {
                        sw.WriteLine(this.textBox3.Text);
                        sw.WriteLine(contrasena);
                        sw.Close();
                    }
                } catch ( Exception e)
                {
                    MessageBox.Show("Ha ocurrido un error" + e.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string borrar = this.comboBox1.Text;
            string lista = "";

            if (File.Exists("..\\bbdd\\" + this.usuario + ".txt"))
            {
                StreamReader sr = new StreamReader("..\\bbdd\\" + this.usuario + ".txt");
                string texto = sr.ReadLine();
                bool encontrado = false;
                while (texto != null)
                {
                    if (texto.Equals(borrar))
                    {
                        encontrado = true;
                    }
                    else
                    {
                        if (encontrado)
                        {
                            encontrado = false;
                        }
                        else
                        {
                            lista += texto + Environment.NewLine;
                        }
                    }
                    texto = sr.ReadLine();
                }
                //close the file
                sr.Close();
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText("..\\bbdd\\" + this.usuario + ".txt"))
                {
                    sw.WriteLine(lista);
                    sw.Close();
                }
                this.comboBox1.Items.Remove(borrar);
                this.comboBox2.Items.Remove(borrar);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            //openFileDialog1.Filter = "(*.xml)";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var path = openFileDialog1.FileName;
                this.label11.Text= path;
            }
            // voy por aquí faltaría la parte de aplicar el descrifrado a la clave
            string clave = obtenerClaveFitx();
            clave = clave.Trim();
            byte[] clave2=clave.Split('-').Select(b => Convert.ToByte(b, 16)).ToArray();

            string clavePlana = decode(clave2);
            this.textBox7.Text = clavePlana;
        }

        private string obtenerClaveFitx()
        {
            string descripcion = this.comboBox2.Text;
            Console.WriteLine(descripcion);
            string clave = "";
            if (File.Exists("..\\bbdd\\"+this.usuario+".txt")) {
                StreamReader sr = new StreamReader("..\\bbdd\\" + this.usuario + ".txt");
                string texto = sr.ReadLine();
                bool encontrado = false;
                while (texto != null)
                {
                    if (texto.Equals(descripcion))
                    {
                        encontrado = true;

                    }  
                    texto = sr.ReadLine();

                    if (encontrado)
                    {
                        encontrado = false;
                        clave = texto;
                        break;

                    }
                }
                //close the file
                sr.Close();
            }
            return clave;
        }
        private string decode(byte [] clave)
        {
            byte[] desencriptado=null;
            using (var rsa = new RSACryptoServiceProvider(2048)) {
                rsa.PersistKeyInCsp = false;

                string privateKey = File.ReadAllText(this.label11.Text);

                rsa.FromXmlString(privateKey);
                try
                {

                    desencriptado = rsa.Decrypt(clave, true);
                }catch (Exception ex)
                {
                    MessageBox.Show("Error al obtener la clave: " + ex.Message);
                }

            }
            if (desencriptado != null)
                return BitConverter.ToString(desencriptado);
            else
                return "Error al generar la clave";
        }
    }
}
