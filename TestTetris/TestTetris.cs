using NoyauTetris;
using Xunit;

namespace TestTetris;

public class TestNoyeau
{
    [Fact]
    public void TestPosition_DeplacementBas_AugmenteY()
    {
        // Arrange
        Position pos = new Position(5, 5);

        // Act
        pos.DeplacerBas();

        // Assert
        Assert.Equal(6, pos.Y);
        Assert.Equal(5, pos.X);
    }

    [Fact]
    public void TestTetrino_InitialisationParDefaut_EstUnCarreRouge()
    {
        // Act
        Tetrino t = new Tetrino();

        // Assert
        Assert.Equal(0, t.Indice); // 0 = Carré
        Assert.Equal(TetrinoCouleur.Rouge, t.Couleur);
        Assert.Equal(0, t.PositionOrigine.X);
        Assert.Equal(0, t.PositionOrigine.Y);
    }

    [Fact]
    public void TestTetrino_Positions_AppliqueLeDecalageOrigine()
    {
        // Arrange
        Tetrino t = new Tetrino();
        t.PositionOrigine = new Position(10, 10);
        t.Indice = 0; // carré 

        // Act
        Position[] posCalculees = t.Positions();

        // Assert
        Assert.Equal(10, posCalculees[0].X);
        Assert.Equal(10, posCalculees[0].Y);
        Assert.Equal(11, posCalculees[1].X);
        Assert.Equal(9, posCalculees[2].Y);
    }

    [Fact]
    public void TestTetrino_NouveauTetrino_GenereCouleurValide()
    {
        // Arrange
        Tetrino t = new Tetrino();

        // Act
        t.NouveauTetrino();

        // Assert
        // Vérifie qu'on n'a pas de Blanc (0) ou Noir (1)
        Assert.True((int)t.Couleur >= 2);
        Assert.True((int)t.Couleur < Enum.GetValues(typeof(TetrinoCouleur)).Length);
    }
}