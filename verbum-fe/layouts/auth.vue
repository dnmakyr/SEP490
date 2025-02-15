<script setup lang="ts"></script>

<template>
  <div
    class="w-full h-screen lg:grid lg:min-h-[600px] lg:grid-cols-2 xl:min-h-[800px]"
  >
    <div class="hidden bg-muted lg:block">
      <div
        class="h-full flex items-center justify-center lg:text-7xl font-bold"
      >
        <div>
          <p>The <span class="text-primary">Localization</span></p>
          <p>platform that easy to use</p>
        </div>
      </div>
    </div>
    <slot />
  </div>
</template>
