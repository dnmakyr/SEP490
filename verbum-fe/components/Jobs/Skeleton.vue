<template>
  <div class="container mx-auto p-4">
      <header class="mb-6 flex justify-between">
        <div class="space-y-2">
          <h1>
            <Skeleton class="w-[968px] max-w-full" />
          </h1>
          <div class="inline-flex items-center border px-2.5 py-0.5 transition-colors border-transparent">
            <Skeleton class="w-[64px] max-w-full" />
          </div>
        </div>
      </header>
      <section class="mb-6">
        <div class="mt-4 space-y-2">
          <div>
            <p>
              <Skeleton class="w-[312px] max-w-full" />
            </p>
            <p>
              <Skeleton class="w-[152px] max-w-full" />
            </p>
            <p>
              <Skeleton class="w-[216px] max-w-full" />
            </p>
            <p>
              <Skeleton class="w-[160px] max-w-full" />
            </p>
            <p>
              <Skeleton class="w-[176px] max-w-full" />
            </p>
            <p>
              <Skeleton class="w-[176px] max-w-full" />
            </p>
          </div>
        </div>
      </section>
      <section class="flex justify-end gap-4 mb-4">
        <div class="inline-flex items-center justify-center transition-colors border border-input h-10 px-4 py-2">
          <Skeleton class="w-[72px] max-w-full" />
        </div>
        <div class="inline-flex items-center justify-center transition-colors border border-input h-10 px-4 py-2">
          <Skeleton class="w-[64px] max-w-full" />
        </div>
      </section>
      <section class="mb-6">
        <div>
          <div class="w-full">
            <div class="h-10 items-center justify-center p-1 grid w-full grid-cols-3">
              <div class="inline-flex items-center justify-center px-3 py-1.5 border">
                <Skeleton class="w-[64px] max-w-full" />
              </div>
              <div class="inline-flex items-center justify-center px-3 py-1.5 border">
                <Skeleton class="w-[80px] max-w-full" />
              </div>
              <div class="inline-flex items-center justify-center px-3 py-1.5 border">
                <Skeleton class="w-[88px] max-w-full" />
              </div>
            </div>
            <div class="mt-2">
              <div class="border h-max-[18rem] overflow-auto">
                <div class="p-2">
                  <a>
                    <Skeleton class="w-[360px] max-w-full" />
                  </a>
                </div>
              </div>
            </div>
            <div class="mt-2"></div>
            <div class="mt-2"></div>
            <div class="mt-2"></div>
          </div>
        </div>
      </section>
    </div>
</template>

<script lang="ts" setup>

</script>

<style>

</style>