using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Nyp.Razor.Spectrum;
using System.Collections;
using System.Threading;
using System.IO.Pipes;
using System.IO;
using System.Windows.Forms;
using Nyp.Razor;
using System.Reflection;

public class JsonData
{
    public string Key;
    public string value;
    public JsonData[] jsonDatas;
    public bool hasChild = false;
}

namespace BTGame
{

    public class Driving : Game
    {
        static NamedPipeClientStream clientStream;
        static StreamWriter writer;

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken;


        TextBox textBoxInstructions;


        TextBox textBox;
        TextBox textBox2;
        TextBox textBox3;
        TextBox textBox4;
        TextBox textBox5;
        TextBox textBox6;
        TextBox textBox7;
        TextBox textBox8;
        TextBox textBox9;
        TextBox textBox10;
        TextBox textBox11;
        TextBox textBox12;
        TextBox textBox13;
        TextBox textBox14;
        TextBox textBox15;

        bool connected = false;

        int counter = 0;


        JsonData EEGJson;
        JsonData EventsJson;
        JsonData KeysJson;

        public static Player Client => Game.m_Clients.Count > 0 ? (Player)Game.m_Clients[0] : (Player)null;


  
        public Driving()
        {
        
        }
        public override void Start_Game()
        {   
            base.Start_Game();

           
        }
        public override void Initialize()
        {
            base.Initialize();

            cancellationToken = cancellationTokenSource.Token;
            DebugForm();


        }

        public bool exeStarted = false;

        public override async void LoadLevel()
        {
           


            base.LoadLevel();
            if (!exeStarted)
            {
                string btSoftwareExePath = AppDomain.CurrentDomain.BaseDirectory;
                string generalFolderPath = btSoftwareExePath.Remove(btSoftwareExePath.LastIndexOf("\\"));
                generalFolderPath = generalFolderPath.Remove(generalFolderPath.LastIndexOf("\\"));

                textBox14.Text = generalFolderPath;
                string exePath = generalFolderPath + @"\Driving";
                exePath += @"\Game.exe";

                Process.Start(exePath);
                exeStarted = true;




                await ClientStreamConnection();
            }
            if (Level == -1 && exeStarted)
                counter++;
            if (IsInSession && exeStarted && Level != -1)
            {
                //writer.WriteLine("!:INTERVAL END");
                EventsJson.jsonDatas[0].value = "false";
            }
        }

        public async Task ClientStreamConnection()
        {

            if (clientStream == null || !clientStream.IsConnected)
            {
                //Create the named pipe client stream and connect
                clientStream = new NamedPipeClientStream(".", "Driving", PipeDirection.Out);
                try
                {
                    await clientStream.ConnectAsync();
                }
                catch (Exception ex) 
                {
                }

                //Create the stream writer
                writer = new StreamWriter(clientStream);

                Console.Beep();
                connected = true; 
                
                InitializeJson();
            }
        }


        public override void Pause_Game()
        {
            base.Pause_Game(); 
            m_bIsPaused = true;
            counter++;
            if (writer != null || !(clientStream == null || !clientStream.IsConnected))
            {
                EventsJson.jsonDatas[1].value = "true";

                //writer.WriteLine("!:PAUSE");
                textBox15.Text = "PAUSE 2";
            }
        }

        public override void Stop_Game()
        {
            base.Stop_Game(); 
        }

        public override void Interval()
        {
            base.Interval();
            counter++;
            textBox15.Text = "Interval";

        }

        public override void Resume_Game()
        {
            if (m_bIsPaused)
            {
                if (counter == 1)
                    base.Resume_Game();
                else if (counter >= 2)
                {
                    OnGamePause(new EventArgs());
                    counter--;
                }
                m_bIsPaused = false;

                if (writer != null || !(clientStream == null || !clientStream.IsConnected))
                {
                    EventsJson.jsonDatas[1].value = "false";

                    //writer.WriteLine("!:RESUME");

                    textBox15.Text = "RESUME 2";
                }

            }
            else if (counter == 1)
            {
                counter--;

                base.Resume_Game();

            }       
        }

        void InitializeJson()
        {
            if(EEGJson != null) { return; }

            EEGJson = new JsonData();
            EEGJson.Key = "EEG";


            EventsJson = new JsonData();
            EventsJson.Key = "Events";
            EventsJson.hasChild = true;

            EventsJson.jsonDatas = new JsonData[2];
            EventsJson.jsonDatas[0] = new JsonData();
            EventsJson.jsonDatas[1] = new JsonData();
            EventsJson.jsonDatas[0].Key = "Interval";
            EventsJson.jsonDatas[1].Key = "Pause";


            KeysJson = new JsonData();
            KeysJson.Key = "Keys";
            KeysJson.hasChild = true;

            KeysJson.jsonDatas = new JsonData[2];
            KeysJson.jsonDatas[0] = new JsonData();
            KeysJson.jsonDatas[1] = new JsonData();
            KeysJson.jsonDatas[0].Key = "Left";
            KeysJson.jsonDatas[1].Key = "Right";
        }

        public override void Update(double elapsed)
        {
            base.Update(elapsed);

            if (connected)
            {
                if (writer != null || !(clientStream == null || !clientStream.IsConnected))
                {


                    KeysJson.jsonDatas[0].value = "false";
                    KeysJson.jsonDatas[1].value = "false";
                    if (Keyboard.IsKeyPressed(37))
                    {
                        textBox.Text = "Key =  Left";
                        KeysJson.jsonDatas[0].value = "true";
                    }
                    if (Keyboard.IsKeyPressed(39))
                    {
                        textBox.Text = "Key =  Right";
                        KeysJson.jsonDatas[1].value = "true";
                    }


                    string eeg;

                    if (Client.EegChannel.A_Trigger) eeg = "true";
                    else eeg = "false";

                    EEGJson.value = eeg;






                    if (!IsInSession && exeStarted && Level == -1)
                    {
                        EventsJson.jsonDatas[0].value = "true";
                    }





                    string json = "{\"" + EEGJson.Key + "\":\"" + EEGJson.value + "\",\""
                        + EventsJson.Key + "\":{\""
                        + EventsJson.jsonDatas[0].Key + "\":\"" + EventsJson.jsonDatas[0].value + "\","
                        + "\"" + EventsJson.jsonDatas[1].Key + "\":\"" + EventsJson.jsonDatas[1].value + "\"},"
                        + "\"" + KeysJson.Key + "\":{"
                        + "\"" + KeysJson.jsonDatas[0].Key + "\":\"" + KeysJson.jsonDatas[0].value + "\","
                        + "\"" + KeysJson.jsonDatas[1].Key + "\":\"" + KeysJson.jsonDatas[1].value + "\"}" +
                        "}" ;


                    try
                    {
                        writer.WriteLine(json);
                        writer.Flush();

                    }
                    catch
                    {
                    }



                }
            }




            ///* Send Values and Thresholds to get variable jump charge speed in the futre

            //writer.WriteLine("A Value : " + Client.EegChannel.A_Value);
            //// Flush the writer and check for IOException (pipe broken)
            //try
            //{
            //    writer.Flush();
            //}
            //catch (IOException ex)
            //{
            //    Exit();
            //}
            
                        textBoxInstructions.Text = "Start EEG Channel first if using, then select level without tabbing out";
            textBoxInstructions.Text = "Start EEG Channel first if using, then select level without tabbing out";
        


            //textBox.Text = "Client.EegChannel.A_Value =  " + Client.EegChannel.A_Value;
            textBox2.Text = "fps =  " + Fps;
            textBox3.Text = "Client.EegChannel.C_Value =  " + Client.EegChannel.C_Value;
            textBox4.Text = "Client.EegChannel.D_Value =  " + Client.EegChannel.D_Value;
            textBox5.Text = "Client.EegChannel.R_Value =  " + Client.EegChannel.R_Value;

           


            textBox6.Text = "Client.EegChannel.R_Threshold =  " + Client.EegChannel.R_Threshold;
            textBox7.Text = "Client.EegChannel.B_Trigger =  " + Client.EegChannel.B_Trigger;
            textBox8.Text = "Client.EegChannel.C_Trigger =  " + Client.EegChannel.C_Trigger;
            textBox9.Text = "Client.EegChannel.D_Trigger =  " + Client.EegChannel.D_Trigger;
            textBox10.Text = "Client.EegChannel.R_Trigger =  " + Client.EegChannel.R_Trigger;


            //textBox11.Text = "Client.EegChannel.A_Threshold =  " + Client.EegChannel.A_Threshold;
            textBox11.Text = "isPause =  " + IsPaused;
           textBox12.Text = "Level =  " + Level; 
            
            textBox13.Text = "coiunter =  " + counter;


            textBox14.Text = "exeStarted =  " + exeStarted;
            //textBox15.Text = "Client.EegChannel.R_Threshold =  " + Client.EegChannel.R_Threshold;
            textBox14.Text = "IsInSession =  " + IsInSession;




        }

        #region Form Creation
        public void DebugForm()
        {
            textBoxInstructions = new TextBox();
            textBoxInstructions.Name = "instructionTextBox";
            textBoxInstructions.Text = "Start dual channel first if using, then select level without tabbing out, selecting level first will force you to tab out and therefore not allow you to enable dual channel";

            textBoxInstructions.Location = new System.Drawing.Point(50, 50);
            textBoxInstructions.Size = new System.Drawing.Size(500, 35);
            textBoxInstructions.Font = new System.Drawing.Font(textBoxInstructions.Font.FontFamily, textBoxInstructions.Font.Size * 1.3f);
            this.Controls.Add(textBoxInstructions);

            textBox = new TextBox();
            // Set properties for the textbox (optional)
            textBox.Name = "dynamicTextBox";
            textBox.Text = "This is a dynamically created textbox.";
            textBox.Location = new System.Drawing.Point(50, 100);
            textBox.Size = new System.Drawing.Size(500, 20);
            // Add the textbox to the form's Controls collection
            this.Controls.Add(textBox);


            textBox2 = new TextBox();
            // Set properties for the textBox2 (optional)
            textBox2.Name = "dynamictextBox2";
            textBox2.Text = "This is a dynamically created textBox2.";
            textBox2.Location = new System.Drawing.Point(50, 125);
            textBox2.Size = new System.Drawing.Size(500, 20);
            // Add the textBox2 to the form's Controls collection
            this.Controls.Add(textBox2);


            textBox3 = new TextBox();
            // Set properties for the textBox2 (optional)
            textBox3.Name = "dynamictextBox3";
            textBox3.Text = "This is a dynamically created textBox2.";
            textBox3.Location = new System.Drawing.Point(50, 150);
            textBox3.Size = new System.Drawing.Size(500, 20);
            // Add the textBox2 to the form's Controls collection
            this.Controls.Add(textBox3);

            textBox4 = new TextBox();
            // Set properties for the textbox (optional)
            textBox4.Name = "dynamicTextBox";
            textBox4.Text = "This is a dynamically created textbox.";
            textBox4.Location = new System.Drawing.Point(50, 175);
            textBox4.Size = new System.Drawing.Size(500, 20);
            // Add the textbox to the form's Controls collection
            this.Controls.Add(textBox4);


            textBox5 = new TextBox();
            // Set properties for the textBox2 (optional)
            textBox5.Name = "dynamictextBox2";
            textBox5.Text = "This is a dynamically created textBox2.";
            textBox5.Location = new System.Drawing.Point(50, 200);
            textBox5.Size = new System.Drawing.Size(500, 20);
            // Add the textBox2 to the form's Controls collection
            this.Controls.Add(textBox5);


            textBox6 = new TextBox();
            // Set properties for the textBox2 (optional)
            textBox6.Name = "dynamictextBox3";
            textBox6.Text = "This is a dynamically created textBox2.";
            textBox6.Location = new System.Drawing.Point(50, 225);
            textBox6.Size = new System.Drawing.Size(500, 20);
            // Add the textBox2 to the form's Controls collection
            this.Controls.Add(textBox6);

            textBox7 = new TextBox();
            // Set properties for the textbox (optional)
            textBox7.Name = "dynamicTextBox";
            textBox7.Text = "This is a dynamically created textbox.";
            textBox7.Location = new System.Drawing.Point(50, 250);
            textBox7.Size = new System.Drawing.Size(500, 20);
            // Add the textbox to the form's Controls collection
            this.Controls.Add(textBox7);


            textBox8 = new TextBox();
            // Set properties for the textBox2 (optional)
            textBox8.Name = "dynamictextBox2";
            textBox8.Text = "This is a dynamically created textBox2.";
            textBox8.Location = new System.Drawing.Point(50, 275);
            textBox8.Size = new System.Drawing.Size(500, 20);
            // Add the textBox2 to the form's Controls collection
            this.Controls.Add(textBox8);


            textBox9 = new TextBox();
            // Set properties for the textBox2 (optional)
            textBox9.Name = "dynamictextBox3";
            textBox9.Text = "This is a dynamically created textBox2.";
            textBox9.Location = new System.Drawing.Point(50, 300);
            textBox9.Size = new System.Drawing.Size(500, 20);
            // Add the textBox2 to the form's Controls collection
            this.Controls.Add(textBox3);

            textBox10 = new TextBox();
            // Set properties for the textbox (optional)
            textBox10.Name = "dynamicTextBox";
            textBox10.Text = "This is a dynamically created textbox.";
            textBox10.Location = new System.Drawing.Point(50, 325);
            textBox10.Size = new System.Drawing.Size(500, 20);
            // Add the textbox to the form's Controls collection
            this.Controls.Add(textBox10);


            textBox11 = new TextBox();
            // Set properties for the textBox2 (optional)
            textBox11.Name = "dynamictextBox2";
            textBox11.Text = "This is a dynamically created textBox2.";
            textBox11.Location = new System.Drawing.Point(50, 350);
            textBox11.Size = new System.Drawing.Size(500, 20);
            // Add the textBox2 to the form's Controls collection
            this.Controls.Add(textBox11);


            textBox12 = new TextBox();
            // Set properties for the textBox2 (optional)
            textBox12.Name = "dynamictextBox3";
            textBox12.Text = "This is a dynamically created textBox2.";
            textBox12.Location = new System.Drawing.Point(50, 400);
            textBox12.Size = new System.Drawing.Size(500, 20);
            // Add the textBox2 to the form's Controls collection
            this.Controls.Add(textBox12);

            textBox13 = new TextBox();
            // Set properties for the textbox (optional)
            textBox13.Name = "dynamicTextBox";
            textBox13.Text = "This is a dynamically created textbox.";
            textBox13.Location = new System.Drawing.Point(50, 425);
            textBox13.Size = new System.Drawing.Size(500, 20);
            // Add the textbox to the form's Controls collection
            this.Controls.Add(textBox13);


            textBox14 = new TextBox();
            // Set properties for the textBox2 (optional)
            textBox14.Name = "dynamictextBox2";
            textBox14.Text = "This is a dynamically created textBox2.";
            textBox14.Location = new System.Drawing.Point(50, 450);
            textBox14.Size = new System.Drawing.Size(500, 20);
            // Add the textBox2 to the form's Controls collection
            this.Controls.Add(textBox14);


            textBox15 = new TextBox();
            // Set properties for the textBox2 (optional)
            textBox15.Name = "dynamictextBox3";
            textBox15.Text = "This is a dynamically created textBox2.";
            textBox15.Location = new System.Drawing.Point(50, 475);
            textBox15.Size = new System.Drawing.Size(500, 20);
            // Add the textBox2 to the form's Controls collection
            this.Controls.Add(textBox15);
        }
        #endregion

      

        protected override void Dispose(bool disposing)
        {
            cancellationToken.ThrowIfCancellationRequested();

            cancellationTokenSource.Cancel();


            //Console.Beep();
     

            if (clientStream != null)
            {
                clientStream.Close();
                clientStream.Dispose();
            }

            base.Dispose(disposing);
        }






        #region EEG Variables
        public override double R_Value_A
        {
            set
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = Game.m_Clients.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return;
                    ((Player)enumerator.Current).EegChannel.R_Value = value;
                }
                finally
                {
                    if (enumerator is IDisposable)
                        ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public override double R_Value_B
        {
            set => this.R_Value_A = value;
        }

        public override double A_Value_A
        {
            set
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = Game.m_Clients.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return;
                    ((Player)enumerator.Current).EegChannel.A_Value = value;
                }
                finally
                {
                    if (enumerator is IDisposable)
                        ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public override double A_Value_B
        {
            set => this.A_Value_A = value;
        }

        public override double B_Value_A
        {
            set
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = Game.m_Clients.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return;
                    ((Player)enumerator.Current).EegChannel.B_Value = value;
                }
                finally
                {
                    if (enumerator is IDisposable)
                        ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public override double B_Value_B
        {
            set => this.B_Value_A = value;
        }

        public override double C_Value_A
        {
            set
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = Game.m_Clients.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return;
                    ((Player)enumerator.Current).EegChannel.C_Value = value;
                }
                finally
                {
                    if (enumerator is IDisposable)
                        ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public override double C_Value_B
        {
            set => this.C_Value_A = value;
        }

        public override double D_Value_A
        {
            set
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = Game.m_Clients.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return;
                    ((Player)enumerator.Current).EegChannel.D_Value = value;
                }
                finally
                {
                    if (enumerator is IDisposable)
                        ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public override double D_Value_B
        {
            set => this.D_Value_A = value;
        }

        public override double R_Threshold_A
        {
            set
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = Game.m_Clients.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return;
                    ((Player)enumerator.Current).EegChannel.R_Threshold = value;
                }
                finally
                {
                    if (enumerator is IDisposable)
                        ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public override double R_Threshold_B
        {
            set => this.R_Threshold_A = value;
        }

        public override double A_Threshold_A
        {
            set
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = Game.m_Clients.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return;
                    ((Player)enumerator.Current).EegChannel.A_Threshold = value;
                }
                finally
                {
                    if (enumerator is IDisposable)
                        ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public override double A_Threshold_B
        {
            set => this.A_Threshold_A = value;
        }

        public override double B_Threshold_A
        {
            set
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = Game.m_Clients.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return;
                    ((Player)enumerator.Current).EegChannel.B_Threshold = value;
                }
                finally
                {
                    if (enumerator is IDisposable)
                        ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public override double B_Threshold_B
        {
            set => this.B_Threshold_A = value;
        }

        public override double C_Threshold_A
        {
            set
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = Game.m_Clients.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return;
                    ((Player)enumerator.Current).EegChannel.C_Threshold = value;
                }
                finally
                {
                    if (enumerator is IDisposable)
                        ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public override double C_Threshold_B
        {
            set => this.C_Threshold_A = value;
        }

        public override double D_Threshold_A
        {
            set
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = Game.m_Clients.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return;
                    ((Player)enumerator.Current).EegChannel.D_Threshold = value;
                }
                finally
                {
                    if (enumerator is IDisposable)
                        ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public override double D_Threshold_B
        {
            set => this.D_Threshold_A = value;
        }

        public override bool R_Trigger_A
        {
            set
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = Game.m_Clients.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return;
                    ((Player)enumerator.Current).EegChannel.R_Trigger = value;
                }
                finally
                {
                    if (enumerator is IDisposable)
                        ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public override bool R_Trigger_B
        {
            set => this.R_Trigger_A = value;
        }

        public override bool A_Trigger_A
        {
            set
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = Game.m_Clients.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return;
                    ((Player)enumerator.Current).EegChannel.A_Trigger = value;
                }
                finally
                {
                    if (enumerator is IDisposable)
                        ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public override bool A_Trigger_B
        {
            set => this.A_Trigger_A = value;
        }

        public override bool B_Trigger_A
        {
            set
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = Game.m_Clients.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return;
                    ((Player)enumerator.Current).EegChannel.B_Trigger = value;
                }
                finally
                {
                    if (enumerator is IDisposable)
                        ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public override bool B_Trigger_B
        {
            set => this.B_Trigger_A = value;
        }

        public override bool C_Trigger_A
        {
            set
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = Game.m_Clients.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return;
                    ((Player)enumerator.Current).EegChannel.C_Trigger = value;
                }
                finally
                {
                    if (enumerator is IDisposable)
                        ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public override bool C_Trigger_B
        {
            set => this.C_Trigger_A = value;
        }

        public override bool D_Trigger_A
        {
            set
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = Game.m_Clients.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return;
                    ((Player)enumerator.Current).EegChannel.D_Trigger = value;
                }
                finally
                {
                    if (enumerator is IDisposable)
                        ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public override bool D_Trigger_B
        {
            set => this.D_Trigger_A = value;
        }

        public override bool R_Ignore_A
        {
            set
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = Game.m_Clients.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return;
                    ((Player)enumerator.Current).EegChannel.R_Ignore = value;
                }
                finally
                {
                    if (enumerator is IDisposable)
                        ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public override bool R_Ignore_B
        {
            set => this.R_Ignore_A = value;
        }

        public override bool A_Ignore_A
        {
            set
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = Game.m_Clients.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return;
                    ((Player)enumerator.Current).EegChannel.A_Ignore = value;
                }
                finally
                {
                    if (enumerator is IDisposable)
                        ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public override bool A_Ignore_B
        {
            set => this.A_Ignore_A = value;
        }

        public override bool B_Ignore_A
        {
            set
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = Game.m_Clients.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return;
                    ((Player)enumerator.Current).EegChannel.B_Ignore = value;
                }
                finally
                {
                    if (enumerator is IDisposable)
                        ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public override bool B_Ignore_B
        {
            set => this.B_Ignore_A = value;
        }

        public override bool C_Ignore_A
        {
            set
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = Game.m_Clients.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return;
                    ((Player)enumerator.Current).EegChannel.C_Ignore = value;
                }
                finally
                {
                    if (enumerator is IDisposable)
                        ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public override bool C_Ignore_B
        {
            set => this.C_Ignore_A = value;
        }

        public override bool D_Ignore_A
        {
            set
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = Game.m_Clients.GetEnumerator();
                    if (!enumerator.MoveNext())
                        return;
                    ((Player)enumerator.Current).EegChannel.D_Ignore = value;
                }
                finally
                {
                    if (enumerator is IDisposable)
                        ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public override bool D_Ignore_B
        {
            set => this.D_Ignore_A = value;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Driving
            // 
            this.Name = "Driving";
            this.Size = new System.Drawing.Size(1048, 768);
            this.ResumeLayout(false);

        }
    }
    #endregion
}







