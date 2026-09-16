# RunePact

## Português (Brasil)

### Visão geral

RunePact é um jogo tático original de cartas em pixel art 2D. O jogador comanda três guerreiros contra três exilados controlados por IA, escolhe cartas de uma mão compartilhada e combina upgrades de equipamentos com decisões em combate por turnos.

Este repositório é a fonte canônica para desenvolvimento local. O marco atual é um protótipo jogável para Windows; ele não contém contas, blockchain, NFTs, compras ou multiplayer online.

O idioma atual para jogadores é somente português brasileiro.

### Recursos do protótipo

Consulte a [evolução de gameplay](Docs/GAMEPLAY-V2.md) para a jornada de três encontros, recompensas, papéis e efeitos.

Consulte também [jornada e decisões](Docs/GAMEPLAY-V3.md) para formações, relíquias, chefe, pausa e salvamento local.

A [direção da interface e dos efeitos](Docs/VISUAL-DESIGN.md) descreve a HUD de fantasia, as cartas em pergaminho e a apresentação do combate.

- Batalha 3 contra 3 com intenções inimigas visíveis.
- Baralho compartilhado, energia, escudo, cura, queimadura, dano em área e dano perfurante.
- Duas escolhas de equipamento por aliado e três níveis de melhoria durante a partida.
- Bônus de Pacto por usar os três aliados em um turno.
- Personagens e arena originais em pixel art gerados para o projeto.

### Início rápido

1. Instale a Unity `6000.5.2f1` com suporte a build Windows.
2. Adicione esta pasta do repositório à Unity Hub.
3. Abra `Assets/Scenes/Battle.unity` e pressione Play.
4. Selecione uma carta e depois um alvo destacado.

A cena cria propositalmente a interface de gameplay durante a execução. A ausência de objetos estáticos de interface antes do Play é esperada.

### Estrutura do projeto

| Caminho | Finalidade |
| --- | --- |
| `Assets/Scripts/Core/` | Regras de combate independentes da engine |
| `Assets/Scripts/Presentation/` | Interface Unity, entrada, animação e som |
| `Assets/Editor/` | Comandos de build e testes de regressão |
| `Assets/Resources/Art/` | Arte em tempo de execução |
| `Docs/` | Documentação bilíngue de projeto, arte e QA |

### Verificações de qualidade

Use **RunePact > Test combat rules** dentro da Unity após mudanças de gameplay. O protótipo inicial passou anteriormente em 2.608 verificações distribuídas por 80 batalhas simuladas com seeds; veja as [notas de QA](Docs/QA.md) para escopo e limitações.

### Regras de contribuição

Mensagens de commit, comentários de código e documentação técnica ou de design são bilíngues em blocos separados: português primeiro e inglês depois. A interface do jogo permanece em português brasileiro até a localização ser introduzida de forma intencional.

Leia [CONTRIBUTING.md](CONTRIBUTING.md), [AGENTS.md](AGENTS.md) e o [guia de direção de arte](Docs/ArtDirection/STYLE-GUIDE.md) antes de modificar o projeto.

---

## English

### Overview

RunePact is an original 2D pixel-art tactical card battler. A player commands three warriors against three AI-controlled exiles, chooses cards from a shared hand, and combines equipment upgrades with turn-based combat decisions.

This repository is the canonical source for local development. The current milestone is a Windows gameplay prototype; it contains no accounts, blockchain, NFTs, purchases, or online multiplayer.

The current player-facing language is Brazilian Portuguese only.

### Prototype features

See [gameplay evolution](Docs/GAMEPLAY-V2.md) for the three-encounter journey, rewards, roles, and effects.

See [journey and decisions](Docs/GAMEPLAY-V3.md) for formations, relics, boss, pause, and local saving.

The [interface and effects direction](Docs/VISUAL-DESIGN.md) describes the fantasy HUD, parchment cards, and combat presentation.

- 3v3 battle with visible enemy intentions.
- Shared deck, energy, shield, healing, burn, area damage, and piercing damage.
- Two equipment choices per ally and three in-match upgrade tiers.
- A Pact bonus for using all three allies in one turn.
- Original generated pixel-art character and arena assets.

### Quick start

1. Install Unity `6000.5.2f1` with Windows Build Support.
2. Add this repository folder to Unity Hub.
3. Open `Assets/Scenes/Battle.unity` and press Play.
4. Select a card and then a highlighted target.

The scene intentionally creates its gameplay UI at runtime. The absence of static UI objects before Play is expected.

### Project structure

| Path | Purpose |
| --- | --- |
| `Assets/Scripts/Core/` | Engine-independent combat rules |
| `Assets/Scripts/Presentation/` | Unity UI, input, animation, and sound |
| `Assets/Editor/` | Build and regression test commands |
| `Assets/Resources/Art/` | Runtime art |
| `Docs/` | Bilingual project, art, and QA documentation |

### Quality checks

Use **RunePact > Test combat rules** inside Unity after gameplay changes. The initial prototype previously passed 2,608 assertions across 80 seeded simulated battles; see the [QA notes](Docs/QA.md) for scope and limitations.

### Contribution rules

Commit messages, code comments, and technical or design documentation are bilingual in separate blocks: Portuguese first, then English. Game UI remains Brazilian Portuguese until localization is intentionally introduced.

Read [CONTRIBUTING.md](CONTRIBUTING.md), [AGENTS.md](AGENTS.md), and the [art direction guide](Docs/ArtDirection/STYLE-GUIDE.md) before modifying the project.
