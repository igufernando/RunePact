# RunePact

## Português (Brasil)

### Visão geral

RunePact é um jogo tático original de cartas em pixel art 2D. O jogador comanda três guerreiros contra três exilados controlados por IA, escolhe cartas de uma mão compartilhada e combina upgrades de equipamentos com decisões em combate por turnos.

Este repositório é a fonte canônica para desenvolvimento local. O marco atual é um protótipo jogável para Windows; ele não contém contas, blockchain, NFTs, compras ou multiplayer online.

O idioma atual para jogadores é somente português brasileiro.

### Recursos do protótipo

Consulte a [rodada visual V6](Docs/VISUAL-V6.md) para cartas ilustradas por habilidade, raridades, itens e animações. A [V5](Docs/GAMEPLAY-V5.md) registra os modos rápido/padrão, dificuldades e cartas exclusivas; a confirmação de alvo foi removida na V6.

Consulte também [jornada e decisões](Docs/GAMEPLAY-V3.md) para formações, relíquias, chefe, pausa e salvamento local.

A [direção da interface e dos efeitos](Docs/VISUAL-DESIGN.md) descreve a HUD de fantasia, as cartas em pergaminho e a apresentação do combate.

- Um ou dois combates 3 contra 3, seguidos pelo Regente de Âmbar, com ordem e intenções inimigas visíveis.
- Baralho compartilhado, energia, escudo, cura, queimadura, dano em área e dano perfurante.
- Duas escolhas de equipamento por aliado e três níveis de melhoria durante a partida.
- Bônus de Pacto por usar os três aliados em um turno.
- Personagens e arena originais em pixel art gerados para o projeto.
- Cartas exclusivas dos três guerreiros, melhorias de habilidade individual, relíquias e inspeção de estados.
- Efeitos persistentes, respiração ancorada nos pés, pausa e velocidade 1×/2×.
- Cartas com poses por habilidade, ícones de função e molduras por raridade; HUD medieval em pixel art.
- Arraste de cartas e uso direto no alvo, catálogo e estatísticas locais de partidas.

### Início rápido

1. Instale a Unity `6000.5.2f1` com suporte a build Windows.
2. Adicione esta pasta do repositório à Unity Hub.
3. Abra `Assets/Scenes/Battle.unity` e pressione Play.
4. Escolha modo e dificuldade. Selecione uma carta e clique no alvo, ou arraste-a até ele: a habilidade é usada imediatamente, sem confirmação extra.

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

Use **RunePact > Test combat rules** dentro da Unity após mudanças de gameplay. Consulte as [notas de QA](Docs/QA.md) para resultados atuais, simulações de balanceamento e limitações.

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

See the [V6 visual milestone](Docs/VISUAL-V6.md) for ability-specific card illustrations, rarities, items, and animations. [V5](Docs/GAMEPLAY-V5.md) records quick/standard modes, difficulties, and unique cards; target confirmation was removed in V6.

See [journey and decisions](Docs/GAMEPLAY-V3.md) for formations, relics, boss, pause, and local saving.

The [interface and effects direction](Docs/VISUAL-DESIGN.md) describes the fantasy HUD, parchment cards, and combat presentation.

- One or two 3v3 battles followed by the Amber Regent, with visible enemy order and intentions.
- Shared deck, energy, shield, healing, burn, area damage, and piercing damage.
- Two equipment choices per ally and three in-match upgrade tiers.
- A Pact bonus for using all three allies in one turn.
- Original generated pixel-art character and arena assets.
- Unique cards for all three warriors, individual ability upgrades, relics, and status inspection.
- Persistent effects, foot-anchored breathing, pause, and 1×/2× speed.
- Ability-specific poses, purpose icons, rarity frames, and a medieval pixel-art HUD.
- Card dragging and direct target use, a catalog, and local run statistics.

### Quick start

1. Install Unity `6000.5.2f1` with Windows Build Support.
2. Add this repository folder to Unity Hub.
3. Open `Assets/Scenes/Battle.unity` and press Play.
4. Choose mode and difficulty. Select a card and click its target, or drag it there: the ability activates immediately without extra confirmation.

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

Use **RunePact > Test combat rules** inside Unity after gameplay changes. See the [QA notes](Docs/QA.md) for current results, balance simulations, and limitations.

### Contribution rules

Commit messages, code comments, and technical or design documentation are bilingual in separate blocks: Portuguese first, then English. Game UI remains Brazilian Portuguese until localization is intentionally introduced.

Read [CONTRIBUTING.md](CONTRIBUTING.md), [AGENTS.md](AGENTS.md), and the [art direction guide](Docs/ArtDirection/STYLE-GUIDE.md) before modifying the project.
