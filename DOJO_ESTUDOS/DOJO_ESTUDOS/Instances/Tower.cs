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
        public IA owner;
        public Vector3 myColor = Vector3.Zero; //trocar dps

        public Tower(Model model, Vector3 initialPosition, float scale)
        {
            this.model = model;
            this.Position = initialPosition;
            this.scale = scale;       
        }

        public void Update(GameTime gt)
        {

            if (owner != null && owner.GetState() == IAState.Dead) 
            {
                owner = null;
                myColor = Vector3.One;
            } 
        }

        public void SetupOwner(IA owner)
        {
            if (owner == null) return;

            this.owner = owner;
            myColor = owner.myColor;
        }

        public float GetScale() { return scale; }

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

                    if(myColor != null)
                    effect.DiffuseColor = myColor;
                }
                mesh.Draw();
            }
        }

    }
}
