----------
## 2020-08-21

Mostrou interesse em fazer algo relacionado com RV, mas não tem ideias.
Marquei para conversar.
Boa Noite Professor!

Então, chegou a hora do famoso TCC I, e eu queria ver se o professor ta disposto a ser meu orientador.

Estou querendo bastante fazer algo de Realidade Virtual, mas ainda não tenho muita idéia de algo que seria válido como TCC, minha ideia era fazer algo em um sistema de RV mais robusto como o Oculus Quest (Portátil) , ou o Oculus Rift S (Desktop). Mandei também uma mensagem no Teams, pode desconsiderar aqui se ver lá, e vice-versa hehe.

Entre as idéias vagas, surgiu uma de um ambiente virtual para algum professor lecionar, com acesso a um quadro branco que ele poderia escrever, o que facilitaria muito pra quem depende de equações, anotações etc, e escrever em RV com um controle é bem mais natural, além de poder ter acesso a diversos a infinitos objetos que poderia interagir pra demonstrar o funcionamento para os alunos.

Mas também gostaria de outas idéias se o professor tiver alguma.

Quando o professor tiver um tempinho e estiver disposto me da um toque dai, pode ser por esse email, pelo email
 
----------
## 2020-08-25

Ok, fechamos a ideia ... usar o Oculus.
Propor uma arquiteura para manipular e explicar conteúdo usando RA.
Ehehe, parece ser um AVA3-RA.
_ Próximos passos:
a) Pensar na arquitetura: escrever alguma coisa e rabiscar um desenho
b) ver qual equipamento vai adquirir: Oculus Quest (Portátil) , ou o Oculus Rift S (Desktop)
c) buscar por exemplos de uso do hardware escolhido (item b) --> trabalhos correlatos
d) propor um título para o "Termo de TCC1"

----------
## 2021-03-03
#### TCC2 - assista
Relatório quinzenais -> prazo aos domingos. 
   sempre ter o relatório pronto na reunião que antecede prazo.

#### Imagens esquemas
- Hardware: o que o Quest oferece: desenvolvedor / usuário

- a sua arquitetura (CFG)
  software 3a mostrar mostrar imagem

- Sala aula virtual: o cenário ideal do professor (montar)
      desenho cenário atuação
    - recursos
      quadro:
      objeto:

=>> bolsista ... trazer para o TCC1 do aquário

#### Pré-teste __
Público
  Assunto: módulo pequeno
  Computação (experimentar): Valdameri / Marcel / Mauricio
  Fora Computação (lecionar): biologia (profa. Roberta)

Teste Final __

----------
## 2021-03-24
Testou rotinas para pegar e fazer uma ação sobre o objeto.
Já tem um quadro (vermelho).
Tentou fazer a rotina para rabiscar direto no quadro.
Achou o Asset para fazer estes rabiscos. Este Asset já tem o palete de cores, espessura, etc.

Não está usando o Trail __
https://docs.unity3d.com/Manual/class-TrailRenderer.html

Vai continuar melhorando as ferramentas do quadro.
Vai tentar deixar o quadro transparente para poder colocar um outro quadro de baixo com um mini-browser.

----------
## 2021-03-31
--> produziu pouco, ficou sem rede ...
Tem um Painel transparente.
Arrumou o RayCasting na ponta da caneta.
Pegou com o Rodrigo (TCC) uma forma diferente de mostrar as mãos virtuais.
.. refinar melhor a caneta e o quadro
.. fazer o navegador

----------
## 2021-04-07
### Browser
#### Asset Browser no Unity
Ver como Alex (bolsista) uso nos projetos.
Orientando achou estes aqui.
https://assetstore.unity.com/packages/tools/gui/3d-webview-for-windows-and-macos-web-browser-154144
https://assetstore.unity.com/packages/tools/gui/3d-webview-for-android-web-browser-137030

Link Projeto Alex: https://github.com/dalton-reis/Unity_WebView

### Sala Virtual

#### Recursos essenciais

Apple PencilKit

#### Softwares para losas digitais

Pesquisar alguns software

### Uso no Quest2

----------

## 2021-04-14

- testou direto usando o Quest 2
- adicionou uma rotina de exemplo que permite selecionar objetos
  - cria uma esfera envolta da pessoa
  - objetos fora da esfera ficam com uma borda preta
  - objetos dentro da esfera ficam sem borda (brancos)
  - apontando para um objeto que pode ser selecionado, e dentro da esfera, fica com uma borda azul
  - agarra o objeto pressionando o botão que fecha os último 3 dedos, e quando solta o botão larga o objeto
- problemas: o risco da caneta fica pipocando .. vai arrumar

----------

## 2021-05-12

- Montou vários objetos da sala.
- Melhorou rotinas de desenho.
- Vai continuar com rotinas de desenho, formas geométricas.
=======

## 2021-05-05

05/05/2021 08:38:19 relatório TCC2
Horas trabalhadas: 320
Muita pesquisa e tutoriais.
Alterada a biblioteca de realidade virtual do projeto do OpenVR para O XRToolkit reestruturando os objetos.
Implementado script próprio de manipulação de objetos em realidade virtual a fim de obter maior controle e um resultado melhor pro meu caso de uso.
Implementado script próprio para manipular a movimentação dos dedos da mão virtual.
Ajustes nos scripts de pintura para se adequar ao uso em realidade virtual.
Melhorias no funcionamento do canetão no quadro branco.
Adicionado cenário.

## 2021-05-19

Vai fazer:

- terminar trocar cor da forma, e mais um forma
- painel de tarefa (não precisar fazer um questionário)

## 2021-05-26

- mais cores na caneta
- acertar os exercícios
- testar alguém próximo
- fazer artigo
