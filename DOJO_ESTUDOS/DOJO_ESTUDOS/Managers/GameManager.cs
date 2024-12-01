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
        public float timerMax = 60f * 2f; // 2 Minutos
        public float timer; 
        public bool HasWinner {get; private set;}
        public bool activeDebug = false;
        public float mapWidth = 0f;
        public float mapHeight = 0f;

        private List<string> nomes_aleatorios = new List<string>
        {
 
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
                return "NOME_GENERICO" + random.Next(9999);

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
            if (HasWinner) return;
          
            List<IA> survivors = new List<IA>();
            IA winner;

            foreach (var ia in ias)
            {
                if (ia.GetHealth() > 0f) survivors.Add(ia);
            }

            if (survivors.Count > 1 && timer < timerMax) return;

            //Desempate por pontos
            if (survivors.Count >= 2)
            {
                survivors.Sort((ia1, ia2) => ia2.GetScore().CompareTo(ia1.GetScore()));
            }

            winner = survivors[0];
            winner.name += " [VENCEDOR] ";
            HasWinner = true;
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
