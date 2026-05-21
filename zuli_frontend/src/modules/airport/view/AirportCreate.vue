<script setup>
import PublicNavBar from '../../../shared/PublicNavBar.vue';
import PublicBottomBar from '../../../shared/PublicBottomBar.vue';
import AirportForm from '../components/AirportForm.vue';
</script>

<template>
    <div class="flex flex-col">
        <PublicNavBar/>
        <main class="flex-1 pb-8">
            <div class="page-shell">
                <AirportForm/>
            </div>
        </main>
    </div>
</template>