using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace arboles_estructura
{
    public partial class Form1 : Form
    {
        private NodoDecision raiz;
        private NodoDecision nodoActual;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CargarArbolDesdeJson();
            MostrarNodo();
        }

        private void CargarArbolDesdeJson()
        {
            string ruta = Path.Combine(Application.StartupPath, "arbol.json");

            if (!File.Exists(ruta))
            {
                MessageBox.Show("No se encontró el archivo 'arbol.json'");
                Application.Exit();
                return;
            }

            string json = File.ReadAllText(ruta);
            NodoDecision arbolCompleto = JsonConvert.DeserializeObject<NodoDecision>(json);

            if (arbolCompleto == null)
            {
                MessageBox.Show("El archivo arbol.json no contiene un árbol válido.");
                Application.Exit();
                return;
            }

            // Obtener solo los nodos que tienen preguntas (que no sean hojas)
            List<NodoDecision> nodosConPreguntas = ObtenerNodosConPreguntas(arbolCompleto);

            if (nodosConPreguntas.Count > 0)
            {
                raiz = nodosConPreguntas[new Random().Next(nodosConPreguntas.Count)];
                nodoActual = raiz;
            }
            else
            {
                MessageBox.Show("No hay suficientes preguntas en el árbol.");
                Application.Exit();
            }
        }

        // Método auxiliar para obtener nodos que tienen preguntas
        private List<NodoDecision> ObtenerNodosConPreguntas(NodoDecision nodo)
        {
            List<NodoDecision> lista = new List<NodoDecision>();
            Queue<NodoDecision> cola = new Queue<NodoDecision>();
            cola.Enqueue(nodo);

            while (cola.Count > 0)
            {
                NodoDecision actual = cola.Dequeue();

                // Solo agregamos nodos que no sean hojas (que tengan pregunta)
                if (!actual.EsHoja)
                {
                    lista.Add(actual);
                }

                if (actual.OpcionSi != null) cola.Enqueue(actual.OpcionSi);
                if (actual.OpcionNo != null) cola.Enqueue(actual.OpcionNo);
            }

            return lista;
        }


        private void MostrarNodo()
        {
            if (nodoActual.EsHoja)
            {
                lbl_pregunta.Text = nodoActual.Recomendacion;
                btn_si.Enabled = false;
                btn_no.Enabled = false;
            }
            else
            {
                lbl_pregunta.Text = nodoActual.Pregunta;
            }
        }

        private void btn_si_Click(object sender, EventArgs e)
        {
            if (nodoActual.OpcionSi != null)
            {
                nodoActual = nodoActual.OpcionSi;
                MostrarNodo();
            }
        }

        private void btn_no_Click(object sender, EventArgs e)
        {
            if (nodoActual.OpcionNo != null)
            {
                nodoActual = nodoActual.OpcionNo;
                MostrarNodo();
            }
        }

        private void btn_reiniciar_Click(object sender, EventArgs e)
        {
            if (nodoActual.EsHoja)
            {
                nodoActual = raiz;
                btn_si.Enabled = true;
                btn_no.Enabled = true;
                MostrarNodo();
            }
            else
            {
                MessageBox.Show("Debes llegar a una recomendación antes de reiniciar.");
            }
        }
    }

    public class NodoDecision
    {
        public string Pregunta { get; set; }
        public string Recomendacion { get; set; }
        public NodoDecision OpcionSi { get; set; }
        public NodoDecision OpcionNo { get; set; }

        public bool EsHoja => OpcionSi == null && OpcionNo == null;
    }
}