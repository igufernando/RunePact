# Regente de Âmbar — português

- Data: 16/09/2026.
- Ferramenta: gerador de imagens integrado, habilidade `imagegen`.
- Referência visual: `Approved-v1/Warriors.png`, inspecionada antes da geração.
- Asset: `Assets/Resources/Art/Regent-v1.png`.
- Status: candidato integrado para avaliação no protótipo; não substitui a base de estilo aprovada.
- Preservação: nenhum personagem do atlas anterior foi redesenhado.

O resultado veio com quadriculado incorporado, não alpha verdadeiro. A tentativa de remoção pelo gerador foi bloqueada por limite de uso. A importação do jogo remove em memória somente o fundo claro conectado às bordas, preservando o arquivo gerado. Em uma etapa futura, produzir alpha limpo de origem antes de ampliar o catálogo.

## Prompt enviado

O prompt original exato em inglês está preservado no bloco abaixo; trata-se de um registro técnico, não de texto para jogadores.

---

# Amber Regent — English

- Date: 2026-09-16.
- Tool: integrated image generator, `imagegen` skill.
- Visual reference: `Approved-v1/Warriors.png`, inspected before generation.
- Asset: `Assets/Resources/Art/Regent-v1.png`.
- Status: integrated candidate for prototype review; does not replace the approved style baseline.
- Preservation: none of the previous atlas characters were redesigned.

The result contained a baked checkerboard rather than true alpha. A generator background-removal retry was blocked by the usage limit. Runtime import removes only light background connected to image edges while preserving the generated file. Clean source alpha should be produced before scaling the catalog.

## Exact submitted prompt

```text
Use case: stylized-concept. Generate ONE new original boss sprite for RunePact. Input image is STYLE REFERENCE ONLY, not edit target: keep exactly its detailed medieval fantasy pixel-art character visual language, blocky metal highlights, dark crisp pixel outlines, three-quarter full body view, comparable pixel density, proportions and material rendering. Subject: an imposing ancient armored Regent with weathered bronze and charcoal segmented armor, moss-green torn cape, a small broken angular crown and a heavy two-handed rune hammer with restrained amber light. Clearly distinct from every reference character. Grounded battle stance facing RIGHT, both boots aligned on the same baseline. ONE character only, centered fully visible including hammer and crown with modest margin. Genuine transparent alpha background, no ground, no scenery, no shadow, no text, no logos, no sheet or panels. Do not redesign reference characters. No smooth painting, vector, 3D, chibi, blur or photorealism. Project-bound asset; preserve reference style.
```
