# RunePact

> **Current player-facing language / Idioma atual para jogadores:** Brazilian Portuguese only / somente português brasileiro.

## Overview / Visão geral

RunePact is an original 2D pixel-art tactical card battler. A player commands three warriors against three AI-controlled exiles, chooses cards from a shared hand, and combines equipment upgrades with turn-based combat decisions.

RunePact é um jogo tático original de cartas em pixel art 2D. O jogador comanda três guerreiros contra três exilados controlados por IA, escolhe cartas de uma mão compartilhada e combina upgrades de equipamentos com decisões em combate por turnos.

This repository is the canonical source for local development. The current milestone is a Windows gameplay prototype; it contains no accounts, blockchain, NFTs, purchases, or online multiplayer.

Este repositório é a fonte canônica para desenvolvimento local. O marco atual é um protótipo jogável para Windows; ele não contém contas, blockchain, NFTs, compras ou multiplayer online.

## Prototype features / Recursos do protótipo

- 3v3 battle with visible enemy intentions / batalha 3 contra 3 com intenções inimigas visíveis.
- Shared deck, energy, shield, healing, burn, area damage, and piercing damage / baralho compartilhado, energia, escudo, cura, queimadura, dano em área e dano perfurante.
- Two equipment choices per ally and three in-match upgrade tiers / duas escolhas de equipamento por aliado e três níveis de melhoria durante a partida.
- A Pact bonus for using all three allies in one turn / bônus de Pacto por usar os três aliados no mesmo turno.
- Original generated pixel-art character and arena assets / personagens e arena originais em pixel art gerados para o projeto.

## Quick start / Início rápido

1. Install Unity `6000.5.2f1` with Windows Build Support / Instale a Unity `6000.5.2f1` com suporte a build Windows.
2. Add this repository folder to Unity Hub / Adicione esta pasta do repositório à Unity Hub.
3. Open `Assets/Scenes/Battle.unity` and press Play / Abra `Assets/Scenes/Battle.unity` e pressione Play.
4. Select a card and then a highlighted target / Selecione uma carta e depois um alvo destacado.

The scene intentionally creates its gameplay UI at runtime. The absence of static UI objects before Play is expected.

A cena cria propositalmente a interface de gameplay durante a execução. A ausência de objetos estáticos de interface antes do Play é esperada.

## Project structure / Estrutura do projeto

| Path / Caminho | Purpose / Finalidade |
| --- | --- |
| `Assets/Scripts/Core/` | Engine-independent combat rules / regras de combate independentes da engine |
| `Assets/Scripts/Presentation/` | Unity UI, input, animation, and sound / interface Unity, entrada, animação e som |
| `Assets/Editor/` | Build and regression test commands / comandos de build e testes de regressão |
| `Assets/Resources/Art/` | Runtime art / arte em tempo de execução |
| `Docs/` | Bilingual project, art, and QA documentation / documentação bilíngue de projeto, arte e QA |

## Quality checks / Verificações de qualidade

Use **RunePact > Test combat rules** inside Unity after gameplay changes. The initial prototype previously passed 2,608 assertions across 80 seeded simulated battles; see [QA notes](Docs/QA.md) for scope and limitations.

Use **RunePact > Test combat rules** dentro da Unity após mudanças de gameplay. O protótipo inicial passou anteriormente em 2.608 verificações distribuídas por 80 batalhas simuladas com seeds; veja [notas de QA](Docs/QA.md) para escopo e limitações.

## Contribution rules / Regras de contribuição

Commit messages, code comments, and technical/design documentation are bilingual: English first, Portuguese second. Game UI remains Brazilian Portuguese until localization is intentionally introduced.

Mensagens de commit, comentários de código e documentação técnica/design são bilíngues: inglês primeiro, português depois. A interface do jogo permanece em português brasileiro até a localização ser introduzida de forma intencional.

Read [CONTRIBUTING.md](CONTRIBUTING.md), [AGENTS.md](AGENTS.md), and the [art direction guide](Docs/ArtDirection/STYLE-GUIDE.md) before modifying the project.

Leia [CONTRIBUTING.md](CONTRIBUTING.md), [AGENTS.md](AGENTS.md) e o [guia de direção de arte](Docs/ArtDirection/STYLE-GUIDE.md) antes de modificar o projeto.
