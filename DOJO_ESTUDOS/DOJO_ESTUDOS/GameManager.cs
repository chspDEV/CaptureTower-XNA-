using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DOJO_ESTUDOS
{
    class GameManager
    {
        public static GameManager Instance;
        public List<IA> ias;
        public Camera camera;

        private List<string> nomes_aleatorios = new List<string>
        {
            "Jorge",
            "Andre",
            "David",
            "Lucy",
            "Pussy",
            "Anitta",
            "JohnPersona",
            "XxDestroyerxX",
            "Megaton",
            "20MATAR70CORRER",
            "PedroTransas",
            "Solid Snake",
            "Cobra solida",
            "Liquid Snake",
            "Venom Snake",
            "Big Boss",
            "Rayden",
            "Tem Sexo nesse jogo?",
            "David Jhones",
            "Podcast",
            "O mago",
            "Monark",
            "Pode pa",
            "Igao",
            "Rownaldinho",
            "Enaldinho",
            "P Diddy",
            "BDSM"
        };


        public GameManager()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException("GameManager já foi instanciado.");
            }

            Instance = this;
        }

        public void Update(GameTime gt)
        { 
            
        }

       
        public string GetRandomName(Random random)
        {
            if (nomes_aleatorios.Count == 0)
                return "NOME_GENERICO" + random.Next(9999);

            int index = random.Next(nomes_aleatorios.Count);
            string selectedName = nomes_aleatorios[index];
            nomes_aleatorios.RemoveAt(index); // Remove o nome para evitar duplicatas
            return selectedName;
        }

    }
}
