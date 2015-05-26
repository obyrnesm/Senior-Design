using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShadowEngine;
using Tao.OpenGl;
using ShadowEngine.OpenGL;
using ShadowEngine.ContentLoading;




using testprogram;
using Database;
//using Driving;
//using test;


namespace CarRace
{
    public partial class MainForm : Form
    {
        uint hdc;
        int selectedCamara = 1;
        int count;
        static int difficulty;
        Controller control = new Controller();
        int shown = 0;
        int moving;
        int time;
        int tcount;


        //LinkedList<double> thisRace = new LinkedList<double>();
        //LinkedList<double> thisSession = new LinkedList<double>();
        
        //thisRace.AddLast(2.0);

        //ArrayList thisRace = new ArrayList();
        double sum = 0;
        double counter = 0;
        double high = 0;
        double low = 100;
        static int userID;

        static float distance = 0; 

        public MainForm()
        {
            InitializeComponent();
            Prog.makeConnection();
            //where to draw identification
            hdc = (uint)this.Handle;
            //error string
            string error = "";
            //Initialize command window graphics
            OpenGLControl.OpenGLInit(ref hdc, this.Width, this.Height, ref error);

            //Iniates camera position and angle perspective,etc etc
            control.Camara.SetPerspective();
            if (error != "")
            {
                MessageBox.Show("Error occured while initializing openGL");
                this.Close();
            }

            comboBox1.Items.Add("Easy");
            comboBox1.Items.Add("Normal");
            comboBox1.Items.Add("Hard");

            //comboBox2.Items.Add("Blue");
            //comboBox2.Items.Add("Red");
            //comboBox2.Items.Add("Black");

            //label2.Text = userID.ToString();
            //Enables lights

            //float[] lightAmbient = { 0.15F, 0.15F, 0.15F, 0.0F };

            //Lighting.LightAmbient = lightAmbient; 

            Lighting.SetupLighting();  // encapsulado en el sahdow engine 
            

            ContentManager.SetTextureList("texturas\\");
            ContentManager.SetModelList("modelos\\");

            ContentManager.LoadTextures();
            ContentManager.LoadModels();

            control.CreateObjects();
            //Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE); 

            //ContentManager.GetModelByName(

            //User user = db_try.getUser(queryUsername, queryPassword);
            //User user = db_try.getUser()

        }


        public static void setUserID(int uID)
        {
            userID = uID;
        }

        public static float getDistance()
        {
            return distance;
        }


        public static void setDifficulty(int diff)
        {
            difficulty = diff;
        }

        public static int getDifficulty()
        {
            return difficulty;
        }
        
        /*
        public static void Update9(double attention)
        {
            label9.Text = attention.ToString();
        }
         * */

        public void UpdateLogic()
        {
           /* if (moving == 1)
            {
                Gl.glTranslatef(0, 0, 0.35f);
            }
            else
                if (moving == -1)
                {
                    Gl.glTranslatef(0, 0, -0.35f);
                }
            * */

            if (Controller.raceOngoing())
            {
                //label2.Text = userID.ToString();

                distance = distance + Player.getSpeed;
                Gl.glTranslatef(0, 0, Player.getSpeed);
                
            }
            
            count++;
            if (Controller.FinishedRace == true && shown == 0)
            {
                shown = 1;
                moving = 0;
                double AverageAtt = sum / counter;
                MessageBox.Show("You finished in: " + lblPrimero.Text
                    //+ Environment.NewLine + "Your Race Time " + time + " Seconds"
                    + Environment.NewLine + "Your Highest Attention: " + high
                    + Environment.NewLine + "Your Lowest Attention: " + low
                    + Environment.NewLine + "Your Average Attention: " + (int)AverageAtt);



                Database.Database db_try = new Database.Database();
                DrivingGame dg = new DrivingGame(userID, time, "");
                db_try.addDrivingGame(dg);
                //min attention, max attention, average attention , min med, max, ave
                ProgressInfo pi = new ProgressInfo(time.ToString(), userID, 0, 0, 0, (int)high, (int)low, (int)AverageAtt);
                db_try.addProgressInfo(pi);
                
            }

            if (count == 10)
            {
                if (Controller.StartedRace == true && shown == 0)
                {
                    int first = control.GetPlayerPlace();
                    //float distanciaRecorrida = control.GetDistanceInMeters(primero);
                    float distanciaRecorrida = control.GetDistanceInMeters();
                    //lblDistancia.Text = Convert.ToString((int)distanciaRecorrida);
                    switch (first)
                    {
                        case 0:
                            {
                                lblPrimero.Text = "First Place";
                                //lblPrimero.ForeColor = Color.Blue;
                                break;
                            }
                        case 1:
                            {
                                lblPrimero.Text = "Second Place";
                                //lblPrimero.ForeColor = Color.Red;
                                break;
                            }
                        case 2:
                            {
                                lblPrimero.Text = "Third Place";
                                //lblPrimero.ForeColor = Color.Black;
                                break;
                            }
                    }
                }
                count = 0;
            }
        }


        private void tmrPaint_Tick(object sender, EventArgs e)
        {
            UpdateLogic(); 
            // clean opengl to draw
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            //draws the entire scene
            control.DrawScene();
            //change buffers
            Winapi.SwapBuffers(hdc);
            //tell opengl to drop any operation he is doing and to prepare for a new frame
            Gl.glFlush(); 
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("You are the Black car." + Environment.NewLine +  "Your Attention controls your speed." + Environment.NewLine +  "Good Luck!");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            selectedCamara--;
            if (selectedCamara == 0)
            {
                selectedCamara = 5;
            }
            lblCamara.Text = Convert.ToString(selectedCamara);
            control.Camara.SelectCamara(selectedCamara - 1);

            label6.Hide();
            label7.Hide();
            label8.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            selectedCamara++;
            if (selectedCamara == 6)
            {
                selectedCamara = 1;
            }
            lblCamara.Text = Convert.ToString(selectedCamara);
            control.Camara.SelectCamara(selectedCamara-1);

            label6.Hide();
            label7.Hide();
            label8.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Controller.StartedRace = true;
            time = 0;
            high = 0;
            low = 100;
            sum = 0;
            counter = 0;
            distance = 0;

            label6.Hide();
            label7.Hide();
            label8.Hide();

        }

        private void btnReiniciar_Click(object sender, EventArgs e)
        {
            lblPrimero.Text = "None";
            //lblDistancia.Text = "0";
            control.ResetRace();
            shown = 0;
            count = 0;
            control.Camara.SelectCamara(selectedCamara-1);
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            Gl.glViewport(0, 0, this.Width, this.Height);
            //select the projection matrix
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            //la reseteo
            Gl.glLoadIdentity();
            //45 = angulo de vision
            //1  = proporcion de alto por ancho
            //0.1f = distancia minima en la que se pinta
            //1000 = distancia maxima
            Glu.gluPerspective(55, this.Width/(float)this.Height  , 0.1f, 1000);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            control.Camara.SelectCamara(selectedCamara - 1); 
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.W)
            {
                moving = 1;
            }
            if (e.KeyData == Keys.S)
            {
                moving = -1;
            }
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            moving = 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label9.Text = Prog.Attention().ToString();
            sum = sum + Prog.Attention();
            counter = counter + 1;

            if (Prog.Attention() > high)
            {
                high = Prog.Attention();
            }
            if (Prog.Attention() < low)
            {
                low = Prog.Attention();
            }
            if (tcount < 9)
            {
                tcount++;
            }
            else
            {
                time = time + 1;
                tcount = 0;
                //label2.Text = time.ToString();
            }
            
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            difficulty = comboBox1.SelectedIndex;
            Car.setDifficulty(difficulty);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            /*
            MainForm game = new MainForm();
            game.Show();
            this.Hide();
             * replace MainForm with the form for the main menu
             */
            //Prog.quit();
            Prog.quit();
            this.Hide();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            //ContentManager.SetTextureList("test2\\");
            //ContentManager.LoadTextures();
            //ContentManager.SetModelList("test1\\");


            //ContentManager.LoadTextures();
            //ContentManager.LoadModels();


        }

        private void button5_Click(object sender, EventArgs e)
        {
            Prog.quit();
            Application.Exit();
        }
    }
}
