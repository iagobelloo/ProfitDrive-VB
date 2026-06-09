# ProfitDrive - Sistema de Gestão e Telemetria para Motoristas de Aplicativo

O ProfitDrive é uma aplicação desktop desenvolvida em VB.NET voltada para o gerenciamento financeiro, controle de custos operacionais e análise de telemetria veicular para motoristas de aplicativo das categorias Comfort e Black.

O projeto foi estruturado no paradigma de Programação Orientada a Objetos (POO), utilizando o Entity Framework Core para a persistência dos dados em um servidor relacional.

## Tecnologias e Arquitetura

- **Linguagem:** VB.NET (Visual Basic .NET).
- **Interface Gráfica:** Windows Forms com renderização customizada via GDI+.
- **Persistência (ORM):** Entity Framework Core (EF Core 9.0).
- **Banco de Dados:** MySQL Server (Provedor Pomelo MySQL), migrado a partir de uma arquitetura inicial em SQLite para suporte a ambiente cliente/servidor rigoroso.
- **Consultas:** Estruturadas via LINQ (Language Integrated Query) com uso de otimizações como consultas desativadas por rastreamento (AsNoTracking).

## Funcionalidades Principais

- **Módulo de Telemetria Operacional:** Identificação automatizada do tipo de motorização (Combustão, Híbrido ou Elétrico), calculando de forma dinâmica médias de consumo por quilômetro rodado, custo operacional por KM e projeção de manutenção preventiva.
- **Controle de Fluxo de Caixa:** Lançamento e conciliação de receitas brutas provenientes das plataformas de transporte em paralelo ao abatimento de custos fixos e variáveis (combustível, despesas fixas e depreciação).
- **Acompanhamento de Metas:** Monitoramento em tempo real do faturamento diário confrontado com a meta estipulada pelo condutor.
- **Business Intelligence (BI):** Módulo de análise estatística avançada para geração de relatórios contábeis executivos.

## Evolução Técnica e Aprendizado (2º Semestre ADS - FATEC)

Desenvolvido como projeto prático no curso de Análise e Desenvolvimento de Sistemas, o software consolidou conceitos essenciais de engenharia de software e banco de dados:
- Transição de persistência local baseada em arquivo (SQLite) para um ambiente baseado em rede com servidor de banco de dados estruturado (MySQL).
- Tratamento de concorrência e gerenciamento de conexões ativas no EF Core, substituindo rotinas legadas de varredura por Reflection por consultas fortemente tipadas e fechamento imediato de fluxos de dados em memória (.ToList()).
- Isolamento de regras de negócio dentro de camadas de dados e entidades relacionais.

## ⚙️ Instalação e Execução (Instruções para Avaliação)

1. Certifique-se de possuir o **MySQL Server** ativo localmente.
2. Abra a solução através do arquivo `ProfitDriveVB.slnx` (ou `.sln`) no Visual Studio.
3. Acesse o arquivo de configuração de contexto do banco (`Data/AppDbContext.vb`) e ajuste as credenciais de `user` e `password` da string de conexão para corresponderem ao seu servidor local.
4. Execute a aplicação. O projeto utiliza o método `db.Database.EnsureCreated()`, gerando a base de dados `profitdrive_v2` e toda a estrutura de tabelas relacionais de forma automática no primeiro carregamento.
