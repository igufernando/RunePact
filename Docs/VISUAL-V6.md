# Rodada visual V6 — Português

## Cartas e interface

- Ilustrações de ação por guerreiro: ataque, defesa e habilidade especial. São variantes da identidade aprovada; os sprites base não foram substituídos.
- Ícones e rótulos distinguem ataque, defesa, cura, suporte, controle e magia. A descrição continua sendo a fonte exata das regras de cada carta.
- Cartas raras usam borda azul e duas runas; épicas usam violeta e três runas, com brilho pulsante. A raridade também aparece por escrito.
- Painéis com cantos recortados em degraus, bordas douradas, relevos e ornamentos rúnicos. Paleta acolhedora de pergaminho, verde e dourado, sem elementos futuristas.
- Relíquias, runa de melhoria, fragmento e baú receberam arte original. A runa representa a melhoria existente, não uma nova moeda.

## Interação e animação

Selecione a carta e clique no alvo, ou arraste até um alvo válido. O uso é imediato, sem a confirmação extra da V5. Alvos inválidos e energia insuficiente continuam bloqueados; Esc desmarca a carta.

Ações combinam pose ilustrada, antecipação, avanço/recuo, inclinação e efeitos de impacto. Proteção usa postura defensiva e cura usa conjuração. As animações são transformações dos sprites com troca de poses, não animação esquelética nem sequências completas quadro a quadro. A posição de repouso é restaurada após a ação.

## Arte e manutenção

`Assets/Resources/Art/WarriorActions-v1.png`: atlas 3×3, linhas Aura/Lyra/Kael; colunas ataque/defesa/especial. `Rewards-v2.png`: atlas 4×2, relíquias na ordem do enum e depois runa, fragmento e baú. Importação Point, sem compressão ou mipmaps e leitura habilitada para recorte por alpha.

Referências e prompts: [registro de arte](ArtDirection/ACTIONS-REWARDS-V1.md). As referências canônicas de `Approved-v1` permanecem inalteradas. Novas gerações devem reutilizar essas referências e preservar a identidade.

---

# Visual milestone V6 — English

## Cards and interface

- Warrior-specific action illustrations: attack, defense, and special ability. These extend the approved identity; base sprites were not replaced.
- Icons and labels distinguish attack, defense, healing, support, control, and magic. Card descriptions remain the exact source for individual rules.
- Rare cards have blue borders and two runes; epic cards use violet and three runes with a pulsing glow. Rarity is also written out.
- Stepped corners, gold frames, bevels, and runic ornaments. The welcoming parchment, green, and gold palette stays free of futuristic elements.
- Relics, upgrade rune, shard, and chest received original artwork. The rune represents the existing upgrade, not a new currency.

## Interaction and animation

Select a card and click its target, or drag it onto a valid target. Use is immediate, without V5's additional confirmation. Invalid targets and insufficient energy remain blocked; Esc deselects the card.

Actions combine illustrated poses, anticipation, lunges/recoil, tilts, and impact effects. Protection uses defensive stances and healing uses casting poses. Animation uses sprite transforms and pose changes, not skeletal animation or complete frame-by-frame sequences. Rest position is restored after the action.

## Art and maintenance

`Assets/Resources/Art/WarriorActions-v1.png`: 3×3 atlas, Aura/Lyra/Kael rows and attack/defense/special columns. `Rewards-v2.png`: 4×2 atlas, relics in enum order followed by rune, shard, and chest. Point filtering, no compression or mipmaps, readable for alpha trimming.

References and prompts: [art record](ArtDirection/ACTIONS-REWARDS-V1.md). Canonical `Approved-v1` references remain unchanged. Future generation must reuse these references and preserve identity.
