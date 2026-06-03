
<template>
    <div class="flex flex-col bg-[#f3f3f3]">
        <main class="flex flex-1 items-center justify-center px-4 py-6 -translate-y-3 sm:px-6">
                <ReserveCard />
            </main>
    </div>
</template>

<script>
import ReserveCard from '../components/ReserveCard.vue';
export default {
    components: {
        ReserveCard
    }
}
</script>