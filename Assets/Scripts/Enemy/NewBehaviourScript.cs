using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invaders : MonoBehaviour
{
    private float xPosition = 0.5f;
    private float yPosition = 0.5f;
    private Vector2 shipPosition;
    private int AccumulatedCost = 0;
    private float gridSize = 7f; // Tama�o de la matriz (7x7)
    private int currentEnergy = 10; // Referenciar a la energia de la nave del jugador.
    public GameObject invasor;
    public GameObject SpaceshipInvaders;

    public int fCost(GameObject Player, GameObject Invader, float X_diff, float Y_diff)//Complete function cost
    {
        Vector2 new_Vector = new Vector2(Player.transform.position.x + X_diff, Player.transform.position.y + Y_diff);

        return (int)(Mathf.Abs(Player.transform.position.x - new_Vector.x) + Mathf.Abs(Player.transform.position.y - new_Vector.y) + Invader.GetComponent<Invaders>().AccumulatedCost);
    }

    void BestMove(GameObject Player, GameObject Invader)
    {
        // Calculamos la nueva posicion (aceptando valores decimales)
        float x_diff = 0;
        float y_diff = 0;

        int CurrentBest = fCost(Player, Invader, 0f, 0f);

        if (xPosition + 1 < gridSize)
        {
            if (fCost(Player, Invader, 1f, 0f) < CurrentBest)
            {
                CurrentBest = fCost(Player, Invader, 1f, 0f);
                x_diff = 1;
                y_diff = 0;
            }
        }
        else if (xPosition - 1 >= 0)
        {
            if (fCost(Player, Invader, -1f, 0f) < CurrentBest)
            {
                CurrentBest = fCost(Player, Invader, -1f, 0f);
                x_diff = -1f;
                y_diff = 0f;
            }

        }
        else if (yPosition + 1 < gridSize)
        {
            if (fCost(Player, Invader, 0f, 1f) < CurrentBest)
            {
                CurrentBest = fCost(Player, Invader, 0f, 1f);
                x_diff = 0f;
                y_diff = 1f;
            }
        }
        else if (yPosition - 1 >= 0)
        {
            if (fCost(Player, Invader, 0f, -1f) < CurrentBest)
            {
                CurrentBest = fCost(Player, Invader, 0f, -1f);
                x_diff = 0f;
                y_diff = -1f;
            }
        }
        Invader.transform.position = new Vector2(Invader.transform.position.x + x_diff, Invader.transform.position.y + y_diff);
        AccumulatedCost += 1;
        // Actualizar la posici�n en el mundo (si tienes un sprite de la nave)
        if (Invader.CompareTag("Spaceship_Player"))
        {
            //Enter scene fight
            Debug.Log("Spaceship_Player detected!");
            //Combate escena es a la que va a salir.
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        int positionX_Invader = Random.Range(0, 7);// 7 es placeholder para energía del invasor.
        int positionY_Invader = 7 - positionX_Invader;
        Vector2 initial_Position = new Vector2(positionX_Invader, positionY_Invader);// Poner invasor a distancia de la energía total inicial del jugador.
        SpaceshipInvaders = Instantiate(invasor, initial_Position, Quaternion.identity);
        //Instanciar con sprite ahí
    }

    void CheckEnergy(SpaceShip PlayerSpaceship, GameObject SpaceshipInvaders)
    {
        if (currentEnergy != PlayerSpaceship.energy)
        {
            Debug.Log("Inside");
            Debug.Log(currentEnergy);
            Debug.Log(PlayerSpaceship.energy);

            BestMove(PlayerSpaceship.gameObject, SpaceshipInvaders);
        }
        Debug.Log("Outside");
        Debug.Log(currentEnergy);
        Debug.Log(PlayerSpaceship.energy);
        currentEnergy = PlayerSpaceship.energy;
    }

    // Update is called once per frame
    void Update()
    {
        SpaceShip playerSpaceship = FindObjectOfType<SpaceShip>();// Find the player spaceship in the scene
        if (playerSpaceship != null)
        {
            Debug.Log("there is a spaceship");
            CheckEnergy(playerSpaceship, SpaceshipInvaders); // Pass the player spaceship reference
        }
    }
}