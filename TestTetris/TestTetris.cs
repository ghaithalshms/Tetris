using NoyauTetris;

namespace TestTetris;

public class TestNoyeau
{
    /** On vérifie que le déplacement en bas augmente la valeur de Y */
    [Fact]
    public void TestPosition_DeplacementBas()
    {
        Position pos = new Position(5, 5);

        pos.DeplacerBas();

        Assert.Equal(6, pos.Y);
        Assert.Equal(5, pos.X);
    }

    /** On vérifie que le déplacement à gauche décrémente bien X */
    [Fact]
    public void TestPosition_DeplacementGauche()
    {
        Position pos = new Position(5, 5);

        pos.DeplacerGauche();

        Assert.Equal(4, pos.X);
        Assert.Equal(5, pos.Y);
    }

    /** On vérifie que le déplacement à gauche décrémente bien X même s'il va sortir du cadre : le noyau ne doit pas gérer cela */
    [Fact]
    public void TestPosition_DeplacementGauche_HorsCadre()
    {
        Position pos = new Position(0, 0);

        pos.DeplacerGauche();

        Assert.Equal(-1, pos.X);
        Assert.Equal(0, pos.Y);
    }

    /** On vérifie que le déplacement à droite incrémente bien X */
    [Fact]
    public void TestPosition_DeplacementDroite()
    {
        Position pos = new Position(5, 5);

        pos.DeplacerDroite();

        Assert.Equal(6, pos.X);
        Assert.Equal(5, pos.Y);
    }

    /** On vérifie que le tetrino par défaut est bien un carré rouge en (0,0) */
    [Fact]
    public void TestTetrino_InitialisationParDefaut()
    {
        Tetrino t = new Tetrino();

        Assert.Equal(0, t.Indice); // 0 = Carré
        Assert.Equal(TetrinoCouleur.Rouge, t.Couleur);
        Assert.Equal(0, t.PositionOrigine.X);
        Assert.Equal(0, t.PositionOrigine.Y);
    }

    /** On vérifie que l'origine est bien en bas à gauche en vérifiant les cordoonées*/
    [Fact]
    public void TestTetrino_Positions()
    {
        Tetrino t = new Tetrino();
        t.PositionOrigine = new Position(10, 10);
        t.Indice = 0; // carré 

        Position[] posCalculees = t.Positions();

        Assert.Equal(10, posCalculees[0].X);
        Assert.Equal(10, posCalculees[0].Y);
        Assert.Equal(11, posCalculees[1].X);
        Assert.Equal(9, posCalculees[2].Y);
    }

    /** On véirife que la couleur générée est valide : l'indice est bien dans la longueur de l'énumération et n'est ni blanc ni noir */
    [Fact]
    public void TestTetrino_NouveauTetrino()
    {
        Tetrino t = new Tetrino();

        t.NouveauTetrino();

        Assert.True((int)t.Couleur >= 2);
        Assert.True((int)t.Couleur < Enum.GetValues(typeof(TetrinoCouleur)).Length);
    }


    /** On vérifie que les positions calculées pour une barre horizontale sont correctes */
    [Fact]
    public void TestTetrino_PositionsBarreHorizontale()
    {
        Tetrino t = new Tetrino();
        t.PositionOrigine = new Position(5, 5);
        t.Indice = 1; // Barre horizontale

        Position[] pos = t.Positions();

        // La barre horizontale fait 4 cases de long : (5,5), (6,5), (7,5), (8,5)
        Assert.Equal(4, pos.Length);
        Assert.Equal(5, pos[0].X);
        Assert.Equal(8, pos[3].X);
        Assert.Equal(5, pos[3].Y);
    }

    /** On vérifie que NouveauTetrino place toujours l'origine X de sorte que 
        le tetrino ne dépasse pas la bordure droite (colonne 12) */
    [Fact]
    public void TestTetrino_ValidationBordureDroite()
    {
        Tetrino t = new Tetrino();

        // On lance le test 50 fois pour couvrir les différents cas aléatoires
        for (int i = 0; i < 50; i++)
        {
            t.NouveauTetrino();
            Position[] pos = t.Positions();

            foreach (var p in pos)
            {
                // Le plateau semble être de 12 colonnes (0 à 11)
                Assert.True(p.X < 12, $"Le tetrino dépasse à droite : X={p.X} pour l'indice {t.Indice}");
                Assert.True(p.X >= 0, $"Le tetrino dépasse à gauche : X={p.X}");
            }
        }
    }

    /** On vérifie que le tableau des couleurs est correctement initialisé dans le constructeur */
    [Fact]
    public void TestTetrino_InitialisationCouleurs()
    {
        Tetrino t = new Tetrino();

        // Vérifie que la première couleur du tableau statique est bien Blanc (0)
        Assert.Equal(TetrinoCouleur.Blanc, Tetrino.CouleursTetrinos[0]);
        // Vérifie que la dernière est bien Cyan (8)
        Assert.Equal(TetrinoCouleur.Cyan, Tetrino.CouleursTetrinos[Tetrino.CouleursTetrinos.Length - 1]);
        // On vérifie qu'ils ont la même longueur 
        Assert.Equal(Tetrino.CouleursTetrinos.Length, Enum.GetValues(typeof(TetrinoCouleur)).Length);
    }
}
