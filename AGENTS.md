# Guia do repositório RunePact (PT-BR)

## Escopo

Este repositório é a fonte local canônica do RunePact. Não desenvolva a partir das cópias anteriores da área de trabalho do Codex. O projeto Tower Defense está pausado e fica fora deste repositório.

## Política de idioma

- A interface do jogo e todo texto para jogadores permanecem em português brasileiro por enquanto.
- Títulos e corpos dos commits, comentários de código, documentação técnica e documentação de design devem ser bilíngues, sempre em blocos separados: primeiro português, depois inglês.
- Mantenha identificadores, nomes de arquivos e símbolos de API em inglês, salvo quando um formato de terceiros exigir o contrário.

## Direção de arte

O estilo aprovado dos personagens está bloqueado até uma mudança explícita do usuário. Leia `Docs/ArtDirection/STYLE-GUIDE.md` e inspecione `Docs/ArtDirection/Approved-v1/` antes de criar ou substituir arte.

## Estrutura do projeto

- `Assets/Scripts/Core/`: regras puras de combate.
- `Assets/Scripts/Presentation/`: interface Unity, entrada e apresentação.
- `Assets/Editor/`: ferramentas de build e testes exclusivas do editor.
- `Assets/Resources/Art/`: arte usada em tempo de execução.
- `Docs/`: documentação de design, arte e QA.

## Verificação

Execute `RunePact > Test combat rules` na Unity após mudanças de gameplay. Não faça commit de `Library`, `Builds`, logs ou arquivos gerados de resultado de testes.

---

# RunePact Repository Guide (EN)

## Scope

This repository is the canonical local source of truth for RunePact. Do not develop from previous Codex workspace copies. The Tower Defense project is paused and is outside this repository.

## Language policy

- Gameplay UI and player-facing text stay in Brazilian Portuguese for now.
- Commit subjects and bodies, code comments, technical documentation, and design documentation must be bilingual in separate blocks: Portuguese first, then English.
- Keep identifiers, file names, and API symbols in English unless a third-party format requires otherwise.

## Art direction

The approved character style is locked until the user explicitly changes it. Read `Docs/ArtDirection/STYLE-GUIDE.md` and inspect `Docs/ArtDirection/Approved-v1/` before creating or replacing artwork.

## Project layout

- `Assets/Scripts/Core/`: pure combat rules.
- `Assets/Scripts/Presentation/`: Unity UI, input, and presentation.
- `Assets/Editor/`: editor-only build and test tooling.
- `Assets/Resources/Art/`: runtime game art.
- `Docs/`: design, art, and QA documentation.

## Verification

Run `RunePact > Test combat rules` in Unity after gameplay changes. Do not commit `Library`, `Builds`, logs, or generated test result files.
