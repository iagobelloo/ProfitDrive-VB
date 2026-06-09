Public Class LancamentoDiario
    Public Property Id As Integer
    Public Property UsuarioId As Integer
    Public Property Data As DateTime
    Public Property KmRodados As Decimal
    Public Property ValorBruto As Decimal
    Public Property TaxaAplicativo As Decimal
    Public Property Origem As String ' Uber, 99, Particular, etc.
    Public Property ValorCombustivel As Decimal
    Public Property Canal As String = ""

    ' Calcula o custo estimado de combustível para este lançamento. Se ValorCombustivel > 0 for preenchido, assume-se que já é o custo real do lançamento.
    Public Function CalcularCustoCombustivel(veiculo As Veiculo) As Decimal
        Try
            If ValorCombustivel > 0D Then
                Return ValorCombustivel
            End If

            If veiculo Is Nothing Then Return 0D

            ' ConsumoCombustivel Km por litro (km/L).
            If veiculo.ConsumoCombustivel <= 0D OrElse veiculo.PrecoCombustivel <= 0D Then
                Return 0D
            End If

            Dim litrosUsados As Decimal = KmRodados / veiculo.ConsumoCombustivel
            Dim custo As Decimal = litrosUsados * veiculo.PrecoCombustivel
            Return Math.Round(custo, 2)
        Catch
            Return 0D
        End Try
    End Function
End Class