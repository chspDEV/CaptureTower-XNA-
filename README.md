# Capture Tower: AI Battle Royale (XNA)

![Linguagem Principal](https://img.shields.io/badge/C%23-100%25-blueviolet)
![Framework](https://img.shields.io/badge/Microsoft%20XNA%204.0-legacy-orange)
![Status](https://img.shields.io/badge/status-arquivado-lightgrey)

> Um projeto de simulação no estilo "Battle Royale" onde múltiplas IAs competem entre si para dominar o cenário. O projeto foi desenvolvido para a matéria de "Inteligência Artificial para Jogos" do curso de Jogos Digitais, utilizando o framework Microsoft XNA.

Este projeto funciona como uma bancada de testes (*testbed*) para IAs, onde a lógica de cada competidor é carregada dinamicamente a partir de arquivos `.lua`, permitindo a prototipagem e o combate rápido entre diferentes comportamentos de IA.

## 🕹️ Mecânicas de Jogo e IA

* **Objetivo:** A IA pode ser programada para vencer de diferentes formas:
    * Acumulando pontos ao capturar torres.
    * Sobrevivendo por mais tempo que as outras (estilo Battle Royale).
    * Eliminando outras IAs.
* **IA Modular:** O comportamento de cada IA é definido em um arquivo `.lua` externo. Isso permite que qualquer pessoa crie sua própria lógica de IA sem precisar recompilar o projeto principal. Os scripts devem ser colocados na pasta `[INFORME AQUI O CAMINHO PARA A PASTA LUA]`.

## 📜 Sobre o Microsoft XNA Framework

O Microsoft XNA foi um framework muito popular para o desenvolvimento de jogos, descontinuado em 2013. Executar este projeto hoje requer um ambiente de desenvolvimento específico, pois não é compatível diretamente com as versões mais recentes do Visual Studio.

## 🛠️ Tecnologias e Arquitetura

-   **Linguagem:** C#
-   **Scripting de IA:** Lua
-   **Framework:** Microsoft XNA Game Studio 4.0
-   **IDE:** Visual Studio 2010

## ⚙️ Como Executar (Requer Ambiente Legado)

**Pré-requisitos:**
-   **Visual Studio:** Visual Studio 2010
-   **Plugin:** Microsoft XNA Game Studio 4.0 (Refresh)
-   **.NET Framework:** 4.0

**Passo a passo:**
1.  Garanta que o ambiente acima esteja configurado corretamente. O XNA 4.0 integra-se nativamente com o Visual Studio 2010.
2.  Clone o repositório: `git clone https://github.com/chspDEV/CaptureTower-XNA-.git`
3.  Abra o arquivo de solução `DOJO_ESTUDOS.sln` com o Visual Studio 2010.
4.  Compile e execute o projeto pressionando `F5`.

## ⌨️ Controles

* **WASD:** Mover a câmera pelo cenário.
* **Shift:** Subir a câmera.
* **Espaço:** Descer a câmera.
* **K:** Ativa as informações de debug.

## 📜 Licença

Este projeto está sob a licença MIT.
