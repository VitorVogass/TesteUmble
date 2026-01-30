
# Desafio Umbler

Esta é uma aplicação web que recebe um domínio e mostra suas informações de DNS.

Este é um exemplo real de sistema que utilizamos na Umbler.

Ex: Consultar os dados de registro do dominio `umbler.com`

**Retorno:**
- Name servers (ns254.umbler.com)
- IP do registro A (177.55.66.99)
- Empresa que está hospedado (Umbler)

Essas informações são descobertas através de consultas nos servidores DNS e de WHOIS.

*Obs: WHOIS (pronuncia-se "ruís") é um protocolo específico para consultar informações de contato e DNS de domínios na internet.*

Nesta aplicação, os dados obtidos são salvos em um banco de dados, evitando uma segunda consulta desnecessaria, caso seu TTL ainda não tenha expirado.

*Obs: O TTL é um valor em um registro DNS que determina o número de segundos antes que alterações subsequentes no registro sejam efetuadas. Ou seja, usamos este valor para determinar quando uma informação está velha e deve ser renovada.*

Tecnologias Backend utilizadas:

- C#
- Asp.Net Core
- MySQL
- Entity Framework

Tecnologias Frontend utilizadas:

- Webpack
- Babel
- ES7

Para rodar o projeto você vai precisar instalar:

- dotnet Core SDK (https://www.microsoft.com/net/download/windows dotnet Core 6.0.201 SDK)
- Um editor de código, acoselhamos o Visual Studio ou VisualStudio Code. (https://code.visualstudio.com/)
- NodeJs v17.6.0 para "buildar" o FrontEnd (https://nodejs.org/en/)
- Um banco de dados MySQL (vc pode rodar localmente ou criar um site PHP gratuitamente no app da Umbler https://app.umbler.com/ que lhe oferece o banco Mysql adicionamente)

Com as ferramentas devidamente instaladas, basta executar os seguintes comandos:

Para "buildar" o javascript basta executar:

`npm install`
`npm run build`

Para Rodar o projeto:

Execute a migration no banco mysql:

`dotnet tool update --global dotnet-ef`
`dotnet tool ef database update`

E após: 

`dotnet run` (ou clique em "play" no editor do vscode)

# Objetivos:

Se você rodar o projeto e testar um domínio, verá que ele já está funcionando. Porém, queremos melhorar varios pontos deste projeto:

# FrontEnd

 - Os dados retornados não estão formatados, e devem ser apresentados de uma forma legível.
 - Não há validação no frontend permitindo que seja submetido uma requsição inválida para o servidor (por exemplo, um domínio sem extensão).
 - Está sendo utilizado "vanilla-js" para fazer a requisição para o backend, apesar de já estar configurado o webpack. O ideal seria utilizar algum framework mais moderno como ReactJs ou Blazor.  

# BackEnd

 - Não há validação no backend permitindo que uma requisição inválida prossiga, o que ocasiona exceptions (erro 500).
 - A complexidade ciclomática do controller está muito alta, o ideal seria utilizar uma arquitetura em camadas.
 - O DomainController está retornando a própria entidade de domínio por JSON, o que faz com que propriedades como Id, Ttl e UpdatedAt sejam mandadas para o cliente web desnecessariamente. Retornar uma ViewModel (DTO) neste caso seria mais aconselhado.

# Testes

 - A cobertura de testes unitários está muito baixa, e o DomainController está impossível de ser testado pois não há como "mockar" a infraestrutura.
 - O Banco de dados já está sendo "mockado" graças ao InMemoryDataBase do EntityFramework, mas as consultas ao Whois e Dns não. 

# Dica

- Este teste não tem "pegadinha", é algo pensado para ser simples. Aconselhamos a ler o código, e inclusive algumas dicas textuais deixadas nos testes unitários. 
- Há um teste unitário que está comentado, que obrigatoriamente tem que passar.
- Diferencial: criar mais testes.

# Entrega

- Enviei o link do seu repositório com o código atualizado.
- O repositório deve estar público para que possamos acessar..
- Modifique Este readme adicionando informações sobre os motivos das mudanças realizadas.

# Modificações:

# Mudanças no Frontend

Substituímos a manipulação direta do DOM pelo React, deixando o código mais organizado e fácil de manter.

Interface dividida em componentes reutilizáveis, com controle de estado usando Hooks.

Validação de domínios no frontend, evitando requisições inválidas para o backend.

Dados da API agora são exibidos de forma clara e legível, mostrando apenas informações válidas do Whois.

Adicionamos feedback visual de carregamento e mensagens de erro para melhorar a experiência do usuário.

Removida a lógica baseada em HTML estático e JavaScript puro.

# Mudanças no Backend

Código reorganizado em camadas (Apresentação, Application, Domain e Infrastructure) para facilitar manutenção e reduzir impactos de mudanças.

Controllers ficaram mais simples: só validam a requisição e delegam a lógica para a camada Application.

Implementada injeção de dependência via interfaces, desacoplando controllers das implementações concretas.

Criado DomainLookupService na camada Application, responsável por consultas e montagem do DTO de resposta.

DTOs na camada Domain evitam o retorno direto de entidades e eliminam dados desnecessários.

Regras de negócio centralizadas nas entidades de domínio, como a checagem de expiração do TTL.

Código duplicado de DNS e Whois foi removido, concentrando essas funções em serviços e adapters.

Adapters na camada Infrastructure encapsulam a comunicação com serviços externos, facilitando manutenção e testes.

Infrastructure organizada entre Data (DbContext, entidades e migrations) e Repositories para acesso a dados.

Sistema mais testável com separação clara entre infraestrutura, domínio e aplicação.

Ajuste no script de migration: após a reorganização, é preciso rodar dotnet ef database update --project ..\Desafio.Umbler.Infrastructure.

Criado dotnet-tools.json para garantir que todos usem a versão correta do dotnet-ef e evitar conflitos de SDK.

# Mudanças nos Testes

Testes reorganizados seguindo a nova arquitetura em camadas.

Novos testes adicionados para regras importantes, como expiração do TTL.

Whois e LookupClient encapsulados em adapters injetáveis, permitindo testes unitários sem depender de serviços externos.

Chamadas duplicadas de DNS e Whois foram unificadas, reduzindo código repetido.

Testes comentados foram ajustados para passar usando mocks, mantendo a validação do comportamento original.

Com a nova estrutura, fica fácil adicionar novos testes mantendo consistência.
