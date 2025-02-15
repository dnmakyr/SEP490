<script setup lang="ts">
import { computed } from 'vue';
import { Check, Languages, PackageCheck, Plus, Truck, X } from 'lucide-vue-next';

const props = defineProps<{ orderStatus: string | undefined }>();

const baseSteps = [
  {
    step: 1,
    title: 'New',
    value: 'NEW',
    description: 'Wait for the center to accept your order',
    icon: Plus,
  },
  {
    step: 2,
    title: 'Accepted',
    value: 'ACCEPTED',
    description: 'Wait for the center to check your files',
    icon: PackageCheck,
  },
  {
    step: 3,
    title: 'In Progress',
    value: 'IN_PROGRESS',
    description: 'The center is working on your files',
    icon: Languages,
  },
  {
    step: 4,
    title: 'Completed',
    value: 'COMPLETED',
    description: 'Your files is ready for your review',
    icon: Check,
  },
  {
    step: 5,
    title: 'Delivered',
    value: 'DELIVERED',
    description: 'Your files has been delivered',
    icon: Truck,
  },
];

const steps = computed(() => {
  return baseSteps.map((step) => {
    if (props.orderStatus === 'REJECTED' && step.step === 2) {
      return {
        step: 2,
        title: 'Rejected',
        value: 'REJECTED',
        description: "You need to update the order according to center's response",
        icon: X,
      };
    }
    return step;
  });
});

const activeStep = computed(() => {
  return steps.value.find((step) => step.value === props.orderStatus)?.step || 1;
});

const isStepAchieved = (step: number) => {
  return step <= activeStep.value;
};
</script>


<template>
  <div class="flex w-full items-start gap-2">
    <div 
      v-for="step in steps"
      :key="step.step"
      class="relative flex w-full flex-col items-center justify-center"
    >
      <Separator 
        v-if="step.step !== steps[steps.length - 1].step"
        class="absolute left-[calc(50%+20px)] right-[calc(-50%+10px)] top-5 block h-0.5 shrink-0 rounded-full bg-muted"
        :class="isStepAchieved(step.step + 1) ? 'bg-primary' : 'bg-muted'"/>
      <Button
        size="icon"
        class="z-10 rounded-full shrink-0 pointer-events-none"
        :class="[
          step.step === activeStep ? 'ring-2 ring-ring ring-offset-2 ring-offset-background' : '',
          (orderStatus === 'REJECTED' && step.step === activeStep) ? 'bg-red-500' : ''
        ]"
        :variant="(isStepAchieved(step.step) ? 'default' : 'outline')"
      >
        <component :is="step.icon" class="size-5" />
      </Button>
      <div class="mt-1 flex flex-col items-center text-center">
        <div
          class="text-sm font-semibold transition lg:text-base"
          :class="[
          step.step === activeStep ? 'text-primary' : '',
          (orderStatus === 'REJECTED' && step.step === activeStep) ? 'text-red-500' : ''
        ]"
        >
          {{ step.title }}
        </div>
        <div
          class="sr-only text-xs text-muted-foreground transition md:not-sr-only lg:text-sm"
          :class="(orderStatus === 'REJECTED' && step.step === activeStep) ? 'text-red-500' : ''"
        >
          {{ step.description }}
        </div>
      </div>
    </div>
  </div>
</template>
