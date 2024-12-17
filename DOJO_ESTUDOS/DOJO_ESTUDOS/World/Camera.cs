using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace DOJO_ESTUDOS
{
    
    public class Camera
    {
        public Vector3 Position { get; private set; }
        public Vector3 Target { get; private set; }

        private float rotationY;
        private float rotationX;
        private float moveSpeed = 0.5f;
        private float rotationSpeed = 0.02f;
        public float renderDistance = 1000f;

        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }

        GraphicsDevice gd;

        public Camera(GraphicsDevice graphicsDevice)
        {
            Position = new Vector3(0, 30, 10);
            Target = Vector3.Zero;
            gd = graphicsDevice;

            UpdateProjectionMatrix(renderDistance);

            UpdateViewMatrix();
        }

        public void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // Movimento para frente e para trás (W e S)
            if (keyboardState.IsKeyDown(Keys.W))
                Position += Vector3.Transform(Vector3.Forward * moveSpeed, Matrix.CreateRotationY(rotationY));
            if (keyboardState.IsKeyDown(Keys.S))
                Position += Vector3.Transform(Vector3.Backward * moveSpeed, Matrix.CreateRotationY(rotationY));

            // Movimento para esquerda e direita (A e D)
            if (keyboardState.IsKeyDown(Keys.A))
                Position += Vector3.Transform(Vector3.Left * moveSpeed, Matrix.CreateRotationY(rotationY));
            if (keyboardState.IsKeyDown(Keys.D))
                Position += Vector3.Transform(Vector3.Right * moveSpeed, Matrix.CreateRotationY(rotationY));

            // Movimento para subir e descer
            if (keyboardState.IsKeyDown(Keys.Space))
                Position += new Vector3(0f, 1f * moveSpeed, 0f);

            if (keyboardState.IsKeyDown(Keys.LeftShift))
                Position -= new Vector3(0f, 1f * moveSpeed, 0f);


            // Rotação horizontal e vertical (Setas Esquerda/Direita e Cima/Baixo)
            if (keyboardState.IsKeyDown(Keys.Left))
                rotationY += rotationSpeed;
            if (keyboardState.IsKeyDown(Keys.Right))
                rotationY -= rotationSpeed;
            if (keyboardState.IsKeyDown(Keys.Up))
                rotationX = MathHelper.Clamp(rotationX + rotationSpeed, -MathHelper.PiOver2, MathHelper.PiOver2);
            if (keyboardState.IsKeyDown(Keys.Down))
                rotationX = MathHelper.Clamp(rotationX - rotationSpeed, -MathHelper.PiOver2, MathHelper.PiOver2);

            //Debug render distance
            if (keyboardState.IsKeyUp(Keys.P))
            {
                renderDistance -= 10f;
                UpdateProjectionMatrix(renderDistance);
            }

            if (keyboardState.IsKeyUp(Keys.O))
            {
                renderDistance += 10f;
                UpdateProjectionMatrix(renderDistance);
            }
                


            UpdateViewMatrix();
        }

        public void ChangePosition(Vector3 newPos)
        {
            Position = newPos;
        }

        private void UpdateProjectionMatrix(float _renderDistance)
        {
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
               MathHelper.ToRadians(45),
               gd.Viewport.AspectRatio,
               0.1f,
               renderDistance
           );
        }

        private void UpdateViewMatrix()
        {
            Matrix rotationMatrix = Matrix.CreateRotationX(rotationX) * Matrix.CreateRotationY(rotationY);
            Vector3 transformedReference = Vector3.Transform(Vector3.Forward, rotationMatrix);

            Target = Position + transformedReference;
            ViewMatrix = Matrix.CreateLookAt(Position, Target, Vector3.Up);
        }
    }

}
