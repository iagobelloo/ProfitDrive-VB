Public Class Usuario
    Public Property Id As Integer
    Public Property Nome As String
    Public Property CPF As String
    Public Property Email As String
    Public Property Endereco As String
    Public Property Senha As String
    Public Property Telefone As String
    Public Property AvatarSelecionado As String
    Public Property MetaDiaria As Decimal
    Public Property CustosFixosMensais As Decimal
    Public Property Ativo As Boolean = True ' True = Acesso Liberado, False = Bloqueado
    Public Property CategoriaPlano As String = "Essential" ' Essential (Grátis), Performance, Black
    Public Property DataVencimentoPlano As DateTime = DateTime.MinValue
    Public Property MensagemAdmin As String = ""
End Class