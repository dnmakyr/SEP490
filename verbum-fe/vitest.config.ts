import { defineVitestConfig } from '@nuxt/test-utils/config'
import { coverageConfigDefaults } from 'vitest/config'

export default defineVitestConfig({
  test: {
    environment: 'nuxt',
    coverage: {
      exclude: [
        '**/components/ui/**',
        '**/app.vue',
        '**/nuxt.config.ts',
        '**/tailwind.config.js',
        ...coverageConfigDefaults.exclude
      ],
      reporter: ['text', 'json-summary', 'json'],
      reportOnFailure: true,
      thresholds: {
        lines: 75,
        branches: 75,
        functions: 75,
        statements: 75
      }
    }
  }
})
