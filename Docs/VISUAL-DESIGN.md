# Interface e efeitos — português

## Direção

A interface usa fantasia acolhedora: pergaminho, verde-sálvia, dourado quente e violeta. Painéis arredondados recebem sombra e bordas em relevo. Cartas usam contraste claro, cor por personagem, custo em medalhão e elevação ao selecionar ou apontar.

O atlas dos personagens e as referências aprovadas permanecem intactos. Formas da interface são criadas em memória por `FantasySkin`; não exigem downloads, fontes externas ou alteração da arte original.

## Leitura de combate

As equipes ocupam formações triangulares espelhadas. Cada unidade tem vida, estados e intenção separados. Os alvos válidos recebem um anel dourado no chão. Energia e Pacto ficam à esquerda das cartas; a ação de encerrar turno fica à direita.

Ataques físicos usam avanço curto, arco de corte e impacto. Projéteis recebem rastro, magias usam partículas e anéis, cura usa folhas, proteção usa um escudo e canalização usa violeta. Dano e escudo mostram a alteração efetiva; cura sem vida recuperada não exibe números positivos falsos. A entrada de novas ações aguarda o término da animação.

## Implementação

- `BattleScreen.cs`: disposição e interação.
- `BattleScreen.Visuals.cs`: superfícies e efeitos temporários.
- `FantasySkin.cs`: formas reutilizáveis e resposta visual dos controles.

O layout usa uma referência de 1600×900. Testes em aparelhos Android e áreas seguras ainda são necessários antes da publicação mobile.

---

# Interface and effects — English

## Direction

The interface uses welcoming fantasy: parchment, sage green, warm gold, and violet. Rounded panels have shadows and beveled edges. Cards use light contrast, character colors, medallion costs, and lift on selection or hover.

The character atlas and approved references remain untouched. Interface shapes are created in memory by `FantasySkin`; no downloads, external fonts, or original artwork changes are required.

## Combat readability

Teams occupy mirrored triangular formations. Each unit has separate health, statuses, and intentions. Valid targets receive a golden ground ring. Energy and Pact appear to the left of the cards; end turn appears to the right.

Physical attacks use a short lunge, slashing arc, and impact. Projectiles have trails, spells use particles and rings, healing uses leaves, protection uses a shield, and channeling uses violet. Damage and shield feedback show actual changes; healing without health recovery does not display false positive numbers. New actions wait until the animation finishes.

## Implementation

- `BattleScreen.cs`: layout and interaction.
- `BattleScreen.Visuals.cs`: surfaces and temporary effects.
- `FantasySkin.cs`: reusable shapes and control feedback.

The layout uses a 1600×900 reference. Android device and safe-area testing are still required before mobile publication.
