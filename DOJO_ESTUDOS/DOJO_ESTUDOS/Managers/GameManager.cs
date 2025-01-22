using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DOJO_ESTUDOS
{
    class GameManager
    {
        public static GameManager Instance;
        public List<IA> ias;
        public List<Tower> towers;
        public Camera camera;
        public float timerMax = 60 * 2f; // 2 Minutos
        public float timer; 
        public bool HasWinner {get; private set;}
        public bool activeDebug = false;
        public float mapWidth = 0f;
        public float mapHeight = 0f;
        public float maxDistanceToMove = 0f;

        private List<string> nomes_aleatorios = new List<string>
        {
            "Claudio",
            "Maisa",
            "Teixeira",
            "Nathan",
            "Carlos barriga mole",
            "CaiozinOCRIA",
            "ArthurMiraTorta",
            "MeioAnao",
            "XupaCus",
            "Safadas",
            "CagadaMole",
            "Ressaca_De_Domingo",
            "Fabio",
            "Serpa_THEGOAT",
            "Marcelo_LEVELUP",
            "AssembleiaDoMarcelo",
            "Gabriel_D_Mago",
            "RodrigoDeputa",
            "Brito_De_Calcinha",
            "Green",
            "Barbabao",
            "Chefao",
            "Pardal Latrocinios & Furtos",
            "Furtado_Foi_Furtado",
            "Lucas_Camisinha",
            "Ervilha",
            "Gemeos",
            "Lairton Mata Cubo",
            "Ate cubanos",
            "TomelheRola",
            "Bon sonaro",
            "Chuta-Cus",
            "Madureira",
            "Duque de Caxias",
            "Serra de Paracambi",
            "Japeri",
            "Os segredos misteriosos de Morro Azul",
            "Feedback TCP 1",
            "Jonas",
            "excel do jonas",
            "indiano arrombado",
            "explodidor de caneta",
            "suvaco de cachorro",
            "amanda cerebro lisin",
            "cala a boca manfro",
            "vai toma no cu joao marcos",
        };

        public GameManager()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException("GameManager já foi instanciado.");
            }

            Instance = this;
            timer = timerMax;
        }

        public void Update(GameTime gt)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.K)) { activeDebug = !activeDebug; }

            if (timer <= 0f && !HasWinner)
            {
                timer = 0f;
                CheckWin();
            }
            else  if (!HasWinner)
            {
                timer -= (float)gt.ElapsedGameTime.TotalSeconds;
            }
        }

        public string GetRandomName(Random random)
        {
            if (nomes_aleatorios.Count == 0)
                return "IA_" + random.Next(9999);

            int index = random.Next(nomes_aleatorios.Count);
            string selectedName = nomes_aleatorios[index];
            nomes_aleatorios.RemoveAt(index); // Remove o nome para evitar duplicatas
            return selectedName;
        }

        public string GetTimerText()
        {
            int minutes = (int)Math.Floor(timer / 60); // Divide o total de segundos por 60 para obter minutos
            int seconds = (int)Math.Floor(timer % 60); // Usa o resto da divisão para obter os segundos

            // Formata a string
            string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);

            string displayText = "Tempo: " + formattedTime;

            return displayText;
        }

        public void CheckWin()
        {
            if (HasWinner) return;  // Se já houver um vencedor, não faz nada.

            List<IA> survivors = new List<IA>();

            // Adiciona todos os IA que ainda têm saúde positiva.
            foreach (var ia in ias)
            {
                if (ia.GetHealth() > 0f) survivors.Add(ia);
            }

            // Se houver apenas 1 sobrevivente ou o tempo acabou, o jogo termina.
            if (survivors.Count == 1 || timer <= 0)
            {
                // Se há pelo menos um sobrevivente, escolhe o vencedor.
                if (survivors.Count > 0)
                {
                    IA winner = survivors[0];  // O vencedor é o único sobrevivente.
                    winner.name += " [VENCEDOR] ";  // Marca o vencedor com o sufixo "[VENCEDOR]".
                    HasWinner = true;  // Define que há um vencedor.
                }
                return;  // Termina a função, pois já há um vencedor ou o tempo acabou.
            }
        }


        public void UpdateIAList(List<IA> i)
        {
            ias = i;
        }

        public void UpdateTowerList(List<Tower> t)
        {
            towers = t;
        }

    }
}
