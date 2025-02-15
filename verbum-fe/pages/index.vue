<script setup lang="ts">
useSeoMeta({
  title: 'VERBUM Translation Center'
})

definePageMeta({
  title: 'Home',
  layout: 'landing',
})

const pricing = [
  {
    name: 'Personal',
    price: 'Translate',
    popular: true,
    features: [
      'Translate the document(s) from one to another language.',
      'This service is suitable for low-visibility, low-importance, and internal content.'
    ],
    button: {
      text: 'Get Started',
      link: '/'
    }
  },
  {
    name: 'Startup',
    price: 'Edit',
    popular: true,
    features: [
      'The existing translations will be reviewed and corrected by a native professional linguist using the source text.',
      'Expect to eliminate the following error types: accuracy, completeness, style, spelling, and grammar.'
    ],
    button: {
      text: 'Get Started',
      link: '#'
    }
  },
  {
    name: 'Enterprise',
    price: 'Evaluate',
    popular: true,
    features: [
      'The existing translations will be reviewed and corrected by a native professional linguist using the translation text only.',
      'Expect to eliminate the following error types: style, spelling, and grammar'
    ],
    button: {
      text: 'Contact us',
      link: '/contact'
    }
  }
]

onMounted(async () => {})
</script>
<template>
  <LandingContainer>
    <LandingHero />
    <div class="mt-16 md:mt-0">
      <h2 class="text-4xl lg:text-5xl font-bold lg:tracking-tight">
        Everything you need to localize your works
      </h2>
      <p class="text-lg mt-4 text-slate-600">
        All services are done by professionals, ensures accurate, culturally sensitive translations for various
        industries.
      </p>
    </div>

    <div class="flex gap-10 w-screen-lg mt-12">
      <LandingPricing v-for="item of pricing" :key="item.name" :plan="item" />
    </div>
    <LandingFooter />
  </LandingContainer>
</template>
