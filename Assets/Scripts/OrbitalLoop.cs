using UnityEngine;

public class OrbitalLoop : MonoBehaviour
{
    [Header("Paramètres de la Boucle")]
    public float radiusX = 5f;       // Largeur de la boucle
    public float radiusZ = 5f;       // Profondeur de la boucle
    public float speed = 2f;         // Vitesse du mouvement

    [Header("Orientation")]
    public bool lookForward = true;  // Est-ce que l'objet regarde vers l'avant ?

    private Vector3 centerPosition;
    private float timer;

    private void Awake()
    {
        // On définit le centre de la boucle là où tu places l'objet au départ
        centerPosition = transform.position;
    }

    private void Update()
    {
        // Le timer avance en continu
        timer += Time.deltaTime * speed;

        // Calcul de la nouvelle position sur le plan XZ (au sol)
        // Pour une boucle parfaite, garde radiusX et radiusZ identiques
        float x = Mathf.Cos(timer) * radiusX;
        float z = Mathf.Sin(timer) * radiusZ;

        Vector3 nextPosition = centerPosition + new Vector3(x, 0, z);

        // Si on veut que l'objet s'oriente vers sa direction de mouvement
        if (lookForward)
        {
            Vector3 direction = nextPosition - transform.position;
            if (direction != Vector3.zero)
            {
                transform.forward = direction;
            }
        }

        // On applique la position
        transform.position = nextPosition;
    }
}