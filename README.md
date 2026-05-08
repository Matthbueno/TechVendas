# TechVendas

This is a challenge by [Coodesh](https://coodesh.com/)

Um sistema de gestão de vendas e controle de estoque para desktop, desenvolvido com foco em arquitetura desacoplada e alta manutenibilidade.

## Tecnologias e Padrões Utilizados

- **Linguagem:** C# (.NET)
- **Interface:** WPF (Windows Presentation Foundation)
- **Persistência:** JSON (Newtonsoft.Json)
- **Arquitetura:** MVVM (Model-View-ViewModel) com separação em camadas:
  - **View:** Interface XAML pura.
  - **ViewModel:** Lógica de apresentação e comandos.
  - **Service:** Camada de domínio e regras de negócio (Validação de CPF, geração de IDs, filtros).
  - **Data:** Camada de infraestrutura e persistência genérica (`DataService<T>`).

## Instalação e Execução

### Opção 1: Instalação via Visual Studio (Recomendado)
1. Faça o clone deste repositório:
   
   git clone https://github.com/Matthbueno/TechVendas.git

2. Navegue até a pasta do projeto e abra o arquivo da Solução (TechVendas.sln) utilizando o Visual Studio.

3. No Visual Studio, compile o projeto (Ctrl + Shift + B) para restaurar os pacotes NuGet necessários.

4. Execute o projeto pressionando F5 (ou clicando no botão "Iniciar").


Opção 2: Configuração Manual dos Componentes de Design
Caso os estilos visuais ou bibliotecas de design (como Material Design) não sejam carregados automaticamente pelo NuGet:

1. Clique com o botão direito na Solução e selecione "Gerenciar Pacotes NuGet para a Solução".

2. Procure por MaterialDesignThemes e Newtonsoft.Json na aba "Instalados" e verifique se as versões estão corretas.

3. Se os recursos de design estiverem em dicionários de recursos externos, verifique se o arquivo App.xaml contém as referências corretas:

XML
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml" />
                <ResourceDictionary Source="pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Dark.xaml" />
                <ResourceDictionary Source="pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.DeepPurple.xaml" />
                <ResourceDictionary Source="pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Secondary/MaterialDesignColor.Lime.xaml" />
                <materialDesign:BundledTheme BaseTheme="Light" PrimaryColor="DeepPurple" SecondaryColor="Lime" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>


Como Usar o Sistema:

- Cadastros: Comece cadastrando Clientes e Produtos nas suas respectivas abas.

- Vendas: Utilize a aba de Pedidos para simular vendas. O sistema permite selecionar clientes preexistentes, adicionar múltiplos itens ao carrinho e escolher a forma de pagamento.

- Persistência: Os dados serão persistidos localmente de forma automática ao clicar em "Salvar".


Persistência de Dados:

- O sistema utiliza arquivos locais para garantir a portabilidade:

- Os dados (Clientes, Produtos e Pedidos) são salvos em formato .json.

- Uma pasta local chamada Data é gerada automaticamente junto ao executável da aplicação (/bin/Debug/net.../Data) durante a primeira execução.

- Esta estrutura permite que o sistema funcione sem a necessidade de um servidor de banco de dados externo configurado.

Notas de Arquitetura:
O projeto foi refatorado para seguir o Princípio de Responsabilidade Única (SRP). As ViewModels não processam dados diretamente; elas delegam as funções para a camada de Services, que por sua vez utiliza 
a camada de Data para o acesso ao disco. Isso permite que, no futuro, o armazenamento em JSON seja substituído por um banco de dados SQL apenas alterando a camada de Infraestrutura.