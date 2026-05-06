
<template>
    <div class="flex flex-col bg-[#f3f3f3]">
        <PublicNavBar />
        <main class="flex flex-1 items-center justify-center px-4 py-6 -translate-y-7 sm:px-6">
                <ReserveCard />
            </main>
    </div>
</template>

<script>
import PublicNavBar from '../components/PublicNavBar.vue';
import PublicBottomBar from '../../../shared/PublicBottomBar.vue';
import ReserveCard from '../components/ReserveCard.vue';
export default {
    components: {
        PublicNavBar,
        PublicBottomBar,
        ReserveCard
    }
}
</script>