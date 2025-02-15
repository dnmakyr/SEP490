import withNuxt from './.nuxt/eslint.config.mjs'

export default withNuxt(
  {
    ignores: [
      'node_modules',
      'dist',
      'public',
      '.nuxt',
      '.output',
      'tests/stores/mockedStore.ts',
      'components/ui'
    ]
  },
  {
    rules: {
      'vue/multi-word-component-names': 'off'
    }
  }
)
