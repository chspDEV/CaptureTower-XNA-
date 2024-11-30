using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DOJO_ESTUDOS
{
    public class Tower
    {
        private Model model;
        public Vector3 Position { get; private set; }
        private Matrix Orientation = Matrix.Identity;
        private float scale;

        public Tower(Model model, Vector3 initialPosition, float scale)
        {
            this.model = model;
            this.Position = initialPosition;
            this.scale = scale;
        }

        public void Update(GameTime gt)
        { 
            
        }

        public void Draw(Matrix view, Matrix projection)
        {
            // Adiciona rotação para corrigir a orientação do modelo
            Matrix rotation = Matrix.CreateRotationX(MathHelper.PiOver2 * 3); //CONTA BOSTA MAS FUNCIONOU PRO MODELO RUIM

            // Matriz de mundo com rotação, escala e translação
            Matrix world = rotation * Orientation * Matrix.CreateScale(scale) * Matrix.CreateTranslation(Position);


            if (model == null) { return; }

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }

    }
}
