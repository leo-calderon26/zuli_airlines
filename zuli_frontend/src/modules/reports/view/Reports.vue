<template>
    <div class="landing-page">
        <AdminNavBar />
                <h1 class="message">Reportes en mantenimiento</h1>
    </div>
</template>

<script>
import AdminNavBar from '../../../shared/AdminNavBar.vue';

export default {
    components: {
        AdminNavBar
    }
}
</script>

<style scoped>
    .landing-page {
        display: flex;
        flex-direction: column;
    }

        .landing-page > :last-child {
            margin-top: auto;
        }

    .message {
        color: #000;
    }
</style>