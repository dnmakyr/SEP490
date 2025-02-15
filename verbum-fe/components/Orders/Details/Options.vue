<template>
  <div class="flex gap-3">
    <Button @click="onClick('Download')">Download</Button>
    <Button v-if="beDisplayed && isEdit" variant="destructive" @click="onClick('Delete')">Delete</Button>
  </div>
</template>

<script lang="ts" setup>
import { useToast } from '~/components/ui/toast';

const props = defineProps<{
  id: string,
  url: string,
  isDeleted?: boolean
  isDelivered?: boolean
  isNewOrRejected?: boolean
  isEdit?: boolean
}>()
const role = inject('role') as string | undefined

const beDisplayed = computed(() => {
  return (!props.isDeleted && !props.isDelivered && role === 'CLIENT' && props.isNewOrRejected)
}
);

const { toast } = useToast()

const onClick = async (options: string) => {
  switch (options) {
    case "Download":
      window.open(props.url, '_blank');
      break;

    case "Delete":
      {
        const { status } = await useAPI("/order/file", {
          method: 'DELETE',
          query: {
            orderId: props.id,
            fileURl: props.url
          }
        });

        if (status.value === "error") {
          toast({
            title: "Can't delete file",
            description: "Try again",
            variant: 'destructive'
          });
        } else if (status.value === "success") {
          toast({
            title: "Deleted successfully",
            description: "File is deleted",
          });
          window.location.reload();
        }
      }
      break;
    case "Recover":
      {
        const { status } = await useAPI("/order/file-recover", {
          method: 'PUT',
          query: {
            orderId: props.id,
            fileURl: props.url
          }
        });

        if (status.value === "error") {
          toast({
            title: "Can't recover file",
            description: "Try again",
            variant: 'destructive'
          });
        } else if (status.value === "success") {
          toast({
            title: "Recovered successfully",
            description: "File is recovered",
          });
          window.location.reload();
        }
      }
      break;

      default:
      console.warn("Unknown option:", options);
      break;
  }
};

</script>