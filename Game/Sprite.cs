using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

internal class Sprite
{
    public Vector2 position;
    public Vector2 direction;
    public Vector2 speed;
    public Texture2D image;
    public bool active = true;
    public bool kicking = false; // Added kicking property
    public float kickTime = 0f;

    private float gravity = 500f;   // The strength of gravity
    private bool jumping = false;   // Whether the sprite is jumping
    private float jumpSpeed = 600f; // The initial speed of the jump
    private float jumpTime = 0f;    // The amount of time the sprite has been jumping
    public float verticalVelocity; // Added verticalVelocity field

    public Rectangle sourceRectangle; // Added sourceRectangle field

    public Sprite(int x, int y, Texture2D image, Rectangle? sourceRectangle = null) // Added optional sourceRectangle parameter
    {
        position = new Vector2(x, y);
        this.image = image;
        this.sourceRectangle = sourceRectangle ?? new Rectangle(0, 0, image.Width, image.Height); // If sourceRectangle is not provided, use the whole image
    }

    // Update method that moves the sprite based on its speed and direction
    public void Update(GameTime gameTime)
    {
        // Check if the sprite is active
        if (active)
        {
            // Move the sprite based on its speed and direction
            position = position + direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Apply gravity force if the sprite is jumping
            if (jumping)
            {
                verticalVelocity -= gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                position.Y -= verticalVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                // If the sprite has been jumping for more than 1 second, stop the jump
                if (jumpTime > 1f)
                {
                    jumping = false;
                    jumpTime = 0f;
                    verticalVelocity = 0f;
                }

                jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }

    public void ApplyGravity(GameTime gameTime)
    {
        // Update the vertical velocity based on the elapsed time and the strength of gravity
        verticalVelocity += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Update the position based on the vertical velocity and the elapsed time
        position.Y += verticalVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Check if the sprite has landed on the ground
        if (position.Y >= 448 - image.Height)
        {
            // Snap the sprite to the ground
            position.Y = 448 - image.Height;

            // Reset the vertical velocity
            verticalVelocity = 0f;

            // Mark the sprite as not jumping
            jumping = false;
        }
    }




    // Collision method that checks if two sprites are colliding
    public bool Collision(Sprite s)
    {
        // Create rectangles around the sprites
        Rectangle r1 = new Rectangle((int)position.X, (int)position.Y, image.Width, image.Height);
        Rectangle r2 = new Rectangle((int)s.position.X, (int)s.position.Y, s.image.Width, s.image.Height);

        // Check if the rectangles intersect, which means the sprites are colliding
        if (r1.Intersects(r2))
        {
            return true;
        }
        return false;
    }


    // UpdateWithWallDetection method that moves the sprite while checking for collisions with walls
    public void UpdateWithWallDetection(GameTime gameTime, Sprite[] wallArray)
    {
        // Create a temporary variable to store the position of the sprite
        Vector2 temp = new Vector2();
        temp = position;

        // Call the Update method to move the sprite
        Update(gameTime);

        // Check for collisions with walls
        for (int i = 0; i < wallArray.Length; i++)
        {
            // Create rectangles around the sprite and the wall
            Rectangle r1;
            Rectangle r2;

            r1 = new Rectangle((int)position.X, (int)position.Y, image.Width, image.Height);
            r2 = new Rectangle((int)wallArray[i].position.X, (int)wallArray[i].position.Y, wallArray[i].image.Width, wallArray[i].image.Height);

            // If the sprite collides with a wall, reset its position to its previous position
            if (r1.Intersects(r2))
            {
                position = temp;

                // Adjust the sprite's position if it is partially inside the wall
                if (position.Y < wallArray[i].position.Y && position.Y + image.Height > wallArray[i].position.Y)
                {
                    position.Y = wallArray[i].position.Y - image.Height;
                }
                else if (position.Y > wallArray[i].position.Y && position.Y < wallArray[i].position.Y + wallArray[i].image.Height)
                {
                    position.Y = wallArray[i].position.Y + wallArray[i].image.Height;
                }
                else if (position.X < wallArray[i].position.X && position.X + image.Width > wallArray[i].position.X)
                {
                    position.X = wallArray[i].position.X - image.Width;
                }
                else if (position.X > wallArray[i].position.X && position.X < wallArray[i].position.X + wallArray[i].image.Width)
                {
                    position.X = wallArray[i].position.X + wallArray[i].image.Width;
                }
            }

        }
    }




    public void Draw(SpriteBatch spriteBatch, Rectangle? sourceRectangle, float scale)
    {
        if (active)
        {
            spriteBatch.Draw(image, position, sourceRectangle, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }

}
