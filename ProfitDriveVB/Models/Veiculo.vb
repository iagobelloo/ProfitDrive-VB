Public Class Veiculo
    Public Property Id As Integer
    Public Property UsuarioId As Integer
    Public Property Modelo As String
    Public Property ConsumoCombustivel As Decimal
    Public Property PrecoCombustivel As Decimal
    Public Property ConsumoEletrico As Decimal
    Public Property PrecoKwh As Decimal
    Public Property TipoVeiculo As String ' "Combustão", "Híbrido" ou "Elétrico"
    Public Property Ativo As Boolean ' Identifica qual carro está usando
End Class